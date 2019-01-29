using System;
using System.Collections.Generic;
using System.Text;
using dg.Utilities;
using AnyGym.core.DAL;
using dg.Sql;
using System.Security.Cryptography;
using dg.Sql.Connector;

namespace AnyGym.core.BL
{
    public static class GymTicketCodeController
    {
        private const Int64 MinCount = 10000;
        private const Int64 AmountToGenerate = 10000000;
        private const int CodeDigits = 6;

        static public string CodeForTicket()
        {
            string result = null;
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                conn.beginTransaction();

                Query qry = new Query(GymTicketCodeBank.TableSchema)
                    .Select(GymTicketCodeBank.Columns.Barcode)
                    .LimitRows(1)
                    .Hint(QueryHint.ForUpdate);
                object code = qry.ExecuteScalar(conn);
                if (!code.IsNull())
                {
                    qry.Delete().Where(GymTicketCodeBank.Columns.Barcode, code).Execute(conn);
                    conn.commitTransaction();
                    result = (string)code;
                }
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                GenerateNextCodeSetIfNeeded();
                return CodeForTicket();
            }
        }

        const string base34Characters = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ"; // No O or I which can confuse
        static string ToBase34(Int64 number, int digits)
        {
            char[] result = new char[digits];
            for (int i = digits - 1; i >= 0; --i)
            {
                result[i] = base34Characters[(int)(number % 34L)];
                number /= 34L;
            }
            return new string(result);
        }
        static Int64 FromBase34(string Base34)
        {
            Int64 result = 0L;
            foreach (char c in Base34)
            {
                result *= 34L;
                result += base34Characters.IndexOf(c);
            }

            return result;
        }
        static public void GenerateNextCodeSetIfNeeded()
        {
            Int64 count = Query.New<GymTicketCodeBank>().GetCount(GymTicketCodeBank.Columns.GymTicketCodeBankId);
            if (count >= MinCount) return;

            Int64 latest = FromBase34((Query.New<GymTicketCodeBankPermanent>().GetMax(GymTicketCodeBank.Columns.Barcode) as string) ?? @"");
            List<Int64> codes = new List<Int64>();

            try
            {
                for (Int64 j = 0; j < AmountToGenerate; j++)
                {
                    latest++;
                    codes.Add(latest);
                }
            }
            catch {}

            Random rng = new Random();
            int n = codes.Count, k;
            Int64 temp;
            while (n > 1) 
            {
                k = rng.Next(n--);
                temp = codes[n];
                codes[n] = codes[k];
                codes[k] = temp;
            }
            
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                conn.beginTransaction();

                Query qry1 = Query.New<GymTicketCodeBank>();
                Query qry2 = Query.New<GymTicketCodeBankPermanent>();

                foreach (Int64 code in codes)
                {
                    qry1.ClearInsertAndUpdate().Insert(GymTicketCodeBank.Columns.Barcode, ToBase34(code, CodeDigits)).Execute(conn);
                    qry2.ClearInsertAndUpdate().Insert(GymTicketCodeBankPermanent.Columns.Barcode, ToBase34(code, CodeDigits)).Execute(conn);
                }

                conn.commitTransaction();
            }
        }
    }
}
