using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;
using dg.Sql;
using dg.Utilities;
using Snoopi.core.DAL;
using Snoopi.core.Caching.Manager;
using System.Globalization;
using dg.Utilities.Apns;
using System.IO;

namespace Snoopi.core.BL
{
    public class CommentUI : Comment
    {
        public string BusinessName { get; set; }
        public string SenderEmail { get; set; }
    }

    public static class CommentsController
    {
        public static List<CommentUI> GetAllCommentsUI(List<Int64> SuppliersIds = null, List<int> StatusIds = null, int PageSize = 0, int CurrentPageIndex = 0)
        {

            Query qry = new Query(Comment.TableSchema);
            qry.Join(JoinType.InnerJoin, Comment.TableSchema, Comment.Columns.SupplierId, Comment.TableSchema.SchemaName,
                AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, Comment.TableSchema, Comment.Columns.AppUserId, Comment.TableSchema.SchemaName,
                AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
          
            qry.OrderBy(Comment.TableSchema.SchemaName, Comment.Columns.Status, SortDirection.ASC); 
            qry.SelectAllTableColumns();
            //qry.SelectAll();
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, "BusinessName");
            qry.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Email, "SenderEmail");
          
            List<CommentUI> list = new List<CommentUI>();
            if (SuppliersIds !=null && SuppliersIds.Count != 0)
            {
                qry.AddWhere(Comment.TableSchema.SchemaName, Comment.Columns.SupplierId, WhereComparision.In, SuppliersIds);
            }
            if (StatusIds != null && StatusIds.Count != 0)
            {
                qry.AddWhere(Comment.TableSchema.SchemaName, Comment.Columns.Status, WhereComparision.In, StatusIds);
            }
            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new CommentUI
                    {
                        CommentId = Convert.ToInt64(reader["CommentId"]),
                        Name = Convert.ToString(reader["Name"]),
                        Rate = Convert.ToInt64(reader["Rate"]),
                        Content = Convert.ToString(reader["Content"]),
                        AppUserId = Convert.ToInt64(reader["AppUserId"]),
                        SupplierId = Convert.ToInt64(reader["SupplierId"]),
                        Status = (CommentStatus)Enum.Parse(typeof(CommentStatus), reader["Status"].ToString()),                      
                        CreateDate = Convert.ToDateTime(reader["CreateDate"]).ToLocalTime(),
                        BusinessName = Convert.ToString(reader["BusinessName"]),
                        SenderEmail = Convert.ToString(reader["SenderEmail"]),
                        BidId = (reader["BidId"] is DBNull ? 0 : Convert.ToInt64(reader["BidId"])),
                        ApproveDate =(reader["ApproveDate"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["ApproveDate"]).ToLocalTime()),
                    });
                }
            }
            return list;
        }


        public static void ApproveComment(Int64 CommentID)
        {
            Comment cmt = Comment.FetchByID(CommentID);
            if (cmt != null)
            {
                cmt.Status = CommentStatus.Approved;
                cmt.ApproveDate = DateTime.UtcNow;
                cmt.Save();
            }

        }

        public static void CancelComment(Int64 CommentID)
        {
            Comment cmt = Comment.FetchByID(CommentID);
            if (cmt != null)
            {
                cmt.Status = CommentStatus.Denied;
                cmt.ApproveDate = null;
                cmt.Save();
            }

        }
    }

  
}
