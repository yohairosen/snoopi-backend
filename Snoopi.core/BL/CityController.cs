using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snoopi.core.DAL;
using dg.Sql;
using dg.Sql.Connector;
using System.IO;

namespace Snoopi.core.BL
{

    public class AreaUI : Area {
        public List<CityUI> Cities { get; set; }
    }
    public class CityUI : City {
        public bool IsSelected { get; set; }
        public string AreaName { get; set; }
    }
    public class CityController
    {


        public static List<AreaUI> GetAllCityUIBy(Int64 SupplierId = 0 , bool IsHomeService = false)
        {
            Query qry = new Query(City.TableSchema);
            qry.Join(JoinType.InnerJoin, Area.TableSchema, Area.TableSchema.SchemaName,
                new JoinColumnPair(City.TableSchema.SchemaName, City.Columns.AreaId, Area.Columns.AreaId));
            if (SupplierId != 0 && !IsHomeService)
            {
                qry.Join(JoinType.LeftJoin, SupplierCity.TableSchema, SupplierCity.TableSchema.SchemaName,
                new JoinColumnPair(City.TableSchema.SchemaName, City.Columns.CityId ,SupplierCity.Columns.CityId),
                new JoinColumnPair(SupplierCity.TableSchema.SchemaName, SupplierCity.Columns.SupplierId, SupplierId,true));
            }
            else if (SupplierId != 0 && IsHomeService)
            {
                qry.Join(JoinType.LeftJoin, SupplierHomeServiceCity.TableSchema, SupplierHomeServiceCity.TableSchema.SchemaName,
                    new JoinColumnPair(City.TableSchema.SchemaName, City.Columns.CityId, SupplierHomeServiceCity.Columns.CityId),
                    new JoinColumnPair(SupplierHomeServiceCity.TableSchema.SchemaName, SupplierHomeServiceCity.Columns.SupplierId, SupplierId,true));
            }

            qry.OrderBy(Area.TableSchema.SchemaName, Area.Columns.AreaName,SortDirection.ASC);
            List<AreaUI> LstCities = new List<AreaUI>();
            Int64 LastArea = 0;
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while(reader.Read())
                {
                    Int64 AreaId= Convert.ToInt64(reader[Area.Columns.AreaId]);
                    if (LastArea == 0) LastArea = AreaId;
                    AreaUI area = null;
                    if (LastArea == AreaId)
                    {
                        area = LstCities.LastOrDefault();
                    }
                    if (area == null)
                    {
                        area = new AreaUI
                        {
                            AreaId = Convert.ToInt64(reader[City.Columns.AreaId]),
                            AreaName = reader[Area.Columns.AreaName] != null ? reader[Area.Columns.AreaName].ToString() : ""
                        };
                        LstCities.Add(area);
                    }
                    if (area.Cities == null) area.Cities = new List<CityUI>();
                    CityUI c = new CityUI
                    {
                        CityId = Convert.ToInt64(reader[City.Columns.CityId]),
                        CityName = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "",
                        AreaId = Convert.ToInt64(reader[City.Columns.AreaId]),
                        IsSelected = SupplierId != 0 && reader[SupplierCity.Columns.SupplierId] != null && reader[SupplierCity.Columns.SupplierId].ToString() != "" ? true : false
                    };
                    area.Cities.Add(c);
                    
                    LastArea = AreaId;
                }
                
            }
            return LstCities;
        }

    }

}

