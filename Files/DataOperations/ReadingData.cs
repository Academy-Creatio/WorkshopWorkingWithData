using System;
using System.Data;
using Terrasoft.Common;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace WorkshopWorkingWithData.Files.DataOperations
{
    internal sealed class ReadingData
    {
        UserConnection UserConnection;
        public ReadingData(UserConnection userConnection)
        {
            UserConnection = userConnection;
        }

        #region Example: GetAllData
        internal Tuple<DataTable, string> GetAllContactsEsq()
        {
            const string tableName = "Contact";
            EntitySchemaQuery esqResult = new EntitySchemaQuery(UserConnection.EntitySchemaManager, tableName);
            esqResult.PrimaryQueryColumn.IsVisible = true;
            
            esqResult.AddColumn("Name");
            esqResult.AddColumn("Email");
            esqResult.AddColumn("Phone");

            Select select = esqResult.GetSelectQuery(UserConnection);
            select.BuildParametersAsValue = true;

            var entities = esqResult.GetEntityCollection(UserConnection);
            return new Tuple<DataTable, string>(entities.ConvertToDataTable(), select.GetSqlText());
        }
        internal Tuple<DataTable, string> GetAllContactsCustomQuery()
        {

            CustomQuery custom = new CustomQuery(UserConnection, "Select Id, Name, Phone, Email from Contact");

            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = custom.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Contact");
                }
            }
            custom.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, custom.GetSqlText());
        }internal Tuple<DataTable, string> GetAllContactsSelect()
        {
            Select select = new Select(UserConnection)
                .Column("Id").As("Id")
                .Column("Contact", "Name").As("ContactName")
                .Column("Email")
                .Column("Phone")
                .From("Contact");


            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = select.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Contact");
                }
            }
            select.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, select.GetSqlText());
        }

        #endregion

        #region Example: GetFilteredData
        internal Tuple<DataTable, string> GetFilteredContactsSelect(string email)
        {
            Select select = new Select(UserConnection)
                .Column("Id").As("Id")
                .Column("Contact", "Name").As("ContactName")
                .Column("Email")
                .Column("Phone")
                .From("Contact")
                .Where("Email").IsEqual(Column.Parameter(email)) as Select;

            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = select.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Contact");
                }
            }
            select.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, select.GetSqlText());
        }

        internal Tuple<DataTable, string> GetFilteredContactsEsq(string email)
        {
            const string tableName = "Contact";
            EntitySchemaQuery esqResult = new EntitySchemaQuery(UserConnection.EntitySchemaManager, tableName);
            esqResult.PrimaryQueryColumn.IsVisible = true;

            esqResult.AddColumn("Name");
            esqResult.AddColumn("Email");
            esqResult.AddColumn("Phone");

            IEntitySchemaQueryFilterItem filterByEmail = esqResult.CreateFilterWithParameters(FilterComparisonType.Equal, "Email", email);
            esqResult.Filters.Add(filterByEmail);

            Select select = esqResult.GetSelectQuery(UserConnection);
            select.BuildParametersAsValue = true;

            var entities = esqResult.GetEntityCollection(UserConnection);
            return new Tuple<DataTable, string>(entities.ConvertToDataTable(), select.GetSqlText());
        }
        internal Tuple<DataTable, string> GetFilteredContactsCustomQuery(string email)
        {
        CustomQuery custom = new CustomQuery(UserConnection, $@"
            Select Id, Name, Phone, Email 
            from Contact
            where Email ='{email}'");

            DataTable dt;

            using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection(QueryKind.General))
            {
                using (IDataReader reader = custom.ExecuteReader(dbExecutor))
                {
                    dt = reader.ReadToDataTable("Contact");
                }
            }
            custom.BuildParametersAsValue = true;
            return new Tuple<DataTable, string>(dt, custom.GetSqlText());
        }
        #endregion
    }
}
