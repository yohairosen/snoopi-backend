using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using Snoopi.core.BL;

namespace Snoopi.core.DAL
{
    public static class DbSchema
    {
        static TableSchema[] Schemas = new TableSchema[] { 
            AppUser.TableSchema,
            AppUserAuthToken.TableSchema,
            User.TableSchema,
            UserAuthToken.TableSchema,
            AppUserConnection.TableSchema,
            AppUserHourlyActivity.TableSchema,
            AppUserMessage.TableSchema,
            AppUserMessageHistory.TableSchema,
            AppUserAPNSToken.TableSchema,
            Category.TableSchema,
            AppUserCategory.TableSchema,
            Group.TableSchema,
            GroupAdmin.TableSchema,
            GroupAdminRequest.TableSchema,
            GroupMember.TableSchema,
            GroupMemberRequest.TableSchema,
            Tremp.TableSchema,
            TrempJoinRequest.TableSchema,
            TrempMember.TableSchema,
            TrempRequest.TableSchema,
            TrempRequestMatch.TableSchema,
            SpatialTremp.TableSchema,
            SpatialTrempRequest.TableSchema,
            UserProfile.TableSchema,
            Permission.TableSchema,
            UserPermissionMap.TableSchema,
            Setting.TableSchema,
            EmailLog.TableSchema,
            EmailTemplate.TableSchema,
            WebErrorLog.TableSchema,
        };
        public static void CreateOrUpgrade()
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                conn.BeginTransaction();

                foreach (TableSchema schema in Schemas)
                {
                    if (!conn.CheckIfTableExists(schema.SchemaName))
                    {
                        try { new Query(schema).CreateTable().Execute(conn); }
                        catch { }
                        try
                        {
                            if (schema.Indexes.Count > 0 || schema.ForeignKeys.Count > 0)
                            {
                                conn.ExecuteScript(new Query(schema).CreateIndexes().ToString());
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        foreach (TableSchema.Column col in schema.Columns)
                        {
                            try { new Query(schema).AddColumn(col).Execute(conn); }
                            catch { }
                            try { new Query(schema).ChangeColumn(col).Execute(conn); }
                            catch { }
                        }
                        foreach (TableSchema.Index idx in schema.Indexes)
                        {
                            try { new Query(schema).CreateIndex(idx).Execute(conn); }
                            catch { }
                        }
                        foreach (TableSchema.ForeignKey key in schema.ForeignKeys)
                        {
                            try { new Query(schema).CreateForeignKey(key).Execute(conn); }
                            catch { }
                        }
                    }
                }

                foreach (string key in Permissions.SystemPermissionKeys)
                {
                    try
                    {
                        if (PermissionCollection.Where(Permission.Columns.Key, key).Count == 0)
                        {
                            new Permission(key).Save(conn);
                        }
                    }
                    catch { }
                }

                string dropTriggers = @"
DROP TRIGGER IF EXISTS tg_trempmember_CountersInsert;
DROP TRIGGER IF EXISTS tg_trempmember_CountersUpdate;
DROP TRIGGER IF EXISTS tg_trempmember_CountersDelete;
DROP TRIGGER IF EXISTS tg_trempjoinrequest_CountersInsert;
DROP TRIGGER IF EXISTS tg_trempjoinrequest_CountersUpdate;
DROP TRIGGER IF EXISTS tg_trempjoinrequest_CountersDelete;
DROP TRIGGER IF EXISTS tg_groupmemberrequest_CountersInsert;
DROP TRIGGER IF EXISTS tg_groupmemberrequest_CountersUpdate;
DROP TRIGGER IF EXISTS tg_groupmemberrequest_CountersDelete;
DROP TRIGGER IF EXISTS tg_groupadminrequest_CountersInsert;
DROP TRIGGER IF EXISTS tg_groupadminrequest_CountersUpdate;
DROP TRIGGER IF EXISTS tg_groupadminrequest_CountersDelete;
DROP TRIGGER IF EXISTS tg_tremprequestmatch_CountersInsert;
DROP TRIGGER IF EXISTS tg_tremprequestmatch_CountersUpdate;
DROP TRIGGER IF EXISTS tg_tremprequestmatch_CountersDelete;
DROP TRIGGER IF EXISTS tg_groupmember_CountersInsert;
DROP TRIGGER IF EXISTS tg_groupmember_CountersUpdate;
DROP TRIGGER IF EXISTS tg_groupmember_CountersDelete;
DROP TRIGGER IF EXISTS tg_tremp_Insert;
DROP TRIGGER IF EXISTS tg_tremp_Update;
DROP TRIGGER IF EXISTS tg_tremp_Delete;
DROP TRIGGER IF EXISTS tg_tremprequest_Insert;
DROP TRIGGER IF EXISTS tg_tremprequest_Update;
DROP TRIGGER IF EXISTS tg_tremprequest_Delete;
";
                string createTriggers = @"
DELIMITER $$

CREATE TRIGGER tg_trempmember_CountersInsert 
AFTER INSERT ON tbl_trempmember
FOR EACH ROW 
BEGIN
	set @TrempId=NEW.TrempId;
	UPDATE tbl_tremp SET BookedSeatsCount = BookedSeatsCount + 1 WHERE TrempId = @TrempId;
END 
$$

CREATE TRIGGER tg_trempmember_CountersUpdate
AFTER UPDATE ON tbl_trempmember
FOR EACH ROW 
BEGIN
	set @TrempId=NEW.TrempId;
	set @OldTrempId=OLD.TrempId;
    IF (@TrempId <> @TrempId) THEN
		UPDATE tbl_tremp SET BookedSeatsCount = BookedSeatsCount - 1 WHERE TrempId = @OldTrempId;
		UPDATE tbl_tremp SET BookedSeatsCount = BookedSeatsCount + 1 WHERE TrempId = @TrempId;
    END IF;
END 
$$

CREATE TRIGGER tg_trempmember_CountersDelete 
AFTER DELETE ON tbl_trempmember
FOR EACH ROW
BEGIN
	set @OldTrempId=OLD.TrempId;
	UPDATE tbl_tremp SET BookedSeatsCount = BookedSeatsCount - 1 WHERE TrempId = @OldTrempId;
END
$$

CREATE TRIGGER tg_trempjoinrequest_CountersInsert 
AFTER INSERT ON tbl_trempjoinrequest
FOR EACH ROW 
BEGIN
	set @TrempId=NEW.TrempId;
	UPDATE tbl_tremp SET MemberRequestsCount = MemberRequestsCount + 1 WHERE TrempId = @TrempId;
END 
$$

CREATE TRIGGER tg_trempjoinrequest_CountersUpdate
AFTER UPDATE ON tbl_trempjoinrequest
FOR EACH ROW 
BEGIN
	set @TrempId=NEW.TrempId;
	set @OldTrempId=OLD.TrempId;
    IF (@TrempId <> @TrempId) THEN
		UPDATE tbl_tremp SET MemberRequestsCount = MemberRequestsCount - 1 WHERE TrempId = @OldTrempId;
		UPDATE tbl_tremp SET MemberRequestsCount = MemberRequestsCount + 1 WHERE TrempId = @TrempId;
    END IF;
END 
$$

CREATE TRIGGER tg_trempjoinrequest_CountersDelete 
AFTER DELETE ON tbl_trempjoinrequest
FOR EACH ROW
BEGIN
	set @OldTrempId=OLD.TrempId;
	UPDATE tbl_tremp SET MemberRequestsCount = MemberRequestsCount - 1 WHERE TrempId = @OldTrempId;
END
$$

CREATE TRIGGER tg_groupmemberrequest_CountersInsert 
AFTER INSERT ON tbl_groupmemberrequest
FOR EACH ROW 
BEGIN
	set @GroupId=NEW.GroupId;
	UPDATE tbl_group SET MemberRequestsCount = MemberRequestsCount + 1 WHERE GroupId = @GroupId;
END 
$$

CREATE TRIGGER tg_groupmemberrequest_CountersUpdate
AFTER UPDATE ON tbl_groupmemberrequest
FOR EACH ROW 
BEGIN
	set @GroupId=NEW.GroupId;
	set @OldGroupId=OLD.GroupId;
    IF (@GroupId <> @GroupId) THEN
		UPDATE tbl_group SET MemberRequestsCount = MemberRequestsCount - 1 WHERE GroupId = @OldGroupId;
		UPDATE tbl_group SET MemberRequestsCount = MemberRequestsCount + 1 WHERE GroupId = @GroupId;
    END IF;
END 
$$

CREATE TRIGGER tg_groupmemberrequest_CountersDelete 
AFTER DELETE ON tbl_groupmemberrequest
FOR EACH ROW
BEGIN
	set @OldGroupId=OLD.GroupId;
	UPDATE tbl_group SET MemberRequestsCount = MemberRequestsCount - 1 WHERE GroupId = @OldGroupId;
END
$$

CREATE TRIGGER tg_groupadminrequest_CountersInsert 
AFTER INSERT ON tbl_groupadminrequest
FOR EACH ROW 
BEGIN
	set @GroupId=NEW.GroupId;
	UPDATE tbl_group SET AdminRequestsCount = AdminRequestsCount + 1 WHERE GroupId = @GroupId;
END 
$$

CREATE TRIGGER tg_groupadminrequest_CountersUpdate
AFTER UPDATE ON tbl_groupadminrequest
FOR EACH ROW 
BEGIN
	set @GroupId=NEW.GroupId;
	set @OldGroupId=OLD.GroupId;
    IF (@GroupId <> @GroupId) THEN
		UPDATE tbl_group SET AdminRequestsCount = AdminRequestsCount - 1 WHERE GroupId = @OldGroupId;
		UPDATE tbl_group SET AdminRequestsCount = AdminRequestsCount + 1 WHERE GroupId = @GroupId;
    END IF;
END 
$$

CREATE TRIGGER tg_groupadminrequest_CountersDelete 
AFTER DELETE ON tbl_groupadminrequest
FOR EACH ROW
BEGIN
	set @OldGroupId=OLD.GroupId;
	UPDATE tbl_group SET AdminRequestsCount = AdminRequestsCount - 1 WHERE GroupId = @OldGroupId;
END
$$

CREATE TRIGGER tg_tremprequestmatch_CountersInsert 
AFTER INSERT ON tbl_tremprequestmatch
FOR EACH ROW 
BEGIN
	set @TrempRequestId=NEW.TrempRequestId;
	UPDATE tbl_tremprequest SET FoundMatchesCount = FoundMatchesCount + 1 WHERE TrempRequestId = @TrempRequestId;
END 
$$

CREATE TRIGGER tg_tremprequestmatch_CountersUpdate
AFTER UPDATE ON tbl_tremprequestmatch
FOR EACH ROW 
BEGIN
	set @TrempRequestId=NEW.TrempRequestId;
	set @OldTrempRequestId=OLD.TrempRequestId;
    IF (@TrempRequestId <> @TrempRequestId) THEN
		UPDATE tbl_tremprequest SET FoundMatchesCount = FoundMatchesCount - 1 WHERE TrempRequestId = @OldTrempRequestId;
		UPDATE tbl_tremprequest SET FoundMatchesCount = FoundMatchesCount + 1 WHERE TrempRequestId = @TrempRequestId;
    END IF;
END 
$$

CREATE TRIGGER tg_tremprequestmatch_CountersDelete 
AFTER DELETE ON tbl_tremprequestmatch
FOR EACH ROW
BEGIN
	set @OldTrempRequestId=OLD.TrempRequestId;
	UPDATE tbl_tremprequest SET FoundMatchesCount = FoundMatchesCount - 1 WHERE TrempRequestId = @OldTrempRequestId;
END
$$

CREATE TRIGGER tg_groupmember_CountersInsert 
AFTER INSERT ON tbl_groupmember
FOR EACH ROW 
BEGIN
	set @GroupId=NEW.GroupId;
	UPDATE tbl_group SET MemberCount = MemberCount + 1 WHERE GroupId = @GroupId;
END 
$$

CREATE TRIGGER tg_groupmember_CountersUpdate
AFTER UPDATE ON tbl_groupmember
FOR EACH ROW 
BEGIN
	set @GroupId=NEW.GroupId;
	set @OldGroupId=OLD.GroupId;
    IF (@GroupId <> @GroupId) THEN
		UPDATE tbl_group SET MemberCount = MemberCount - 1 WHERE GroupId = @OldGroupId;
		UPDATE tbl_group SET MemberCount = MemberCount + 1 WHERE GroupId = @GroupId;
    END IF;
END 
$$

CREATE TRIGGER tg_groupmember_CountersDelete 
AFTER DELETE ON tbl_groupmember
FOR EACH ROW
BEGIN
	set @OldGroupId=OLD.GroupId;
	UPDATE tbl_group SET MemberCount = MemberCount - 1 WHERE GroupId = @OldGroupId;
END
$$

CREATE TRIGGER tg_tremp_Insert 
AFTER INSERT ON tbl_Tremp
FOR EACH ROW 
BEGIN
	SET @TrempId=NEW.TrempId;
	SET @FromAddressLatitude=NEW.FromAddressLatitude;
	SET @FromAddressLongitude=NEW.FromAddressLongitude;
	SET @ToAddressLatitude=NEW.ToAddressLatitude;
	SET @ToAddressLongitude=NEW.ToAddressLongitude;
    IF (@FromAddressLatitude IS NOT NULL AND @FromAddressLongitude IS NOT NULL AND @ToAddressLatitude IS NOT NULL AND @ToAddressLongitude IS NOT NULL) THEN
        INSERT INTO tbl_SpatialTremp (TrempId, FromAddress, ToAddress) VALUES(@TrempId,POINT(@FromAddressLatitude,@FromAddressLongitude),POINT(@ToAddressLatitude,@ToAddressLongitude));
	END IF;
END 
$$

CREATE TRIGGER tg_tremp_Update
AFTER UPDATE ON tbl_Tremp
FOR EACH ROW 
BEGIN
	SET @TrempId=NEW.TrempId;
	SET @FromAddressLatitude=NEW.FromAddressLatitude;
	SET @FromAddressLongitude=NEW.FromAddressLongitude;
	SET @ToAddressLatitude=NEW.ToAddressLatitude;
	SET @ToAddressLongitude=NEW.ToAddressLongitude;
	SET @OldTrempId=OLD.TrempId;
	SET @OldFromAddressLatitude=OLD.FromAddressLatitude;
	SET @OldFromAddressLongitude=OLD.FromAddressLongitude;
	SET @OldToAddressLatitude=OLD.ToAddressLatitude;
	SET @OldToAddressLongitude=OLD.ToAddressLongitude;
	IF (@TrempId <> @OldTrempId OR @FromAddressLatitude <> @OldFromAddressLatitude OR @FromAddressLongitude <> @OldFromAddressLongitude OR @ToAddressLatitude <> @OldToAddressLatitude OR @ToAddressLongitude <> @OldToAddressLongitude) THEN
        DELETE FROM tbl_SpatialTremp WHERE TrempId = @OldTrempId;
        IF (@FromAddressLatitude IS NOT NULL AND @FromAddressLongitude IS NOT NULL AND @ToAddressLatitude IS NOT NULL AND @ToAddressLongitude IS NOT NULL) THEN
            INSERT INTO tbl_SpatialTremp (TrempId, FromAddress, ToAddress) VALUES(@TrempId,POINT(@FromAddressLatitude,@FromAddressLongitude),POINT(@ToAddressLatitude,@ToAddressLongitude));
	    END IF;
	END IF;
END 
$$

CREATE TRIGGER tg_tremp_Delete 
AFTER DELETE ON tbl_Tremp
FOR EACH ROW
BEGIN
	SET @OldTrempId=OLD.TrempId;
    DELETE FROM tbl_SpatialTremp WHERE TrempId = @OldTrempId;
END
$$

CREATE TRIGGER tg_tremprequest_Insert 
AFTER INSERT ON tbl_TrempRequest
FOR EACH ROW 
BEGIN
	SET @TrempRequestId=NEW.TrempRequestId;
	SET @FromAddressLatitude=NEW.FromAddressLatitude;
	SET @FromAddressLongitude=NEW.FromAddressLongitude;
	SET @ToAddressLatitude=NEW.ToAddressLatitude;
	SET @ToAddressLongitude=NEW.ToAddressLongitude;
    IF (@FromAddressLatitude IS NOT NULL AND @FromAddressLongitude IS NOT NULL AND @ToAddressLatitude IS NOT NULL AND @ToAddressLongitude IS NOT NULL) THEN
        INSERT INTO tbl_SpatialTrempRequest (TrempRequestId, FromAddress, ToAddress) VALUES(@TrempRequestId,POINT(@FromAddressLatitude,@FromAddressLongitude),POINT(@ToAddressLatitude,@ToAddressLongitude));
	END IF;
END 
$$

CREATE TRIGGER tg_tremprequest_Update
AFTER UPDATE ON tbl_TrempRequest
FOR EACH ROW 
BEGIN
	SET @TrempRequestId=NEW.TrempRequestId;
	SET @FromAddressLatitude=NEW.FromAddressLatitude;
	SET @FromAddressLongitude=NEW.FromAddressLongitude;
	SET @ToAddressLatitude=NEW.ToAddressLatitude;
	SET @ToAddressLongitude=NEW.ToAddressLongitude;
	SET @OldTrempRequestId=OLD.TrempRequestId;
	SET @OldFromAddressLatitude=OLD.FromAddressLatitude;
	SET @OldFromAddressLongitude=OLD.FromAddressLongitude;
	SET @OldToAddressLatitude=OLD.ToAddressLatitude;
	SET @OldToAddressLongitude=OLD.ToAddressLongitude;
	IF (@TrempRequestId <> @OldTrempRequestId OR @FromAddressLatitude <> @OldFromAddressLatitude OR @FromAddressLongitude <> @OldFromAddressLongitude OR @ToAddressLatitude <> @OldToAddressLatitude OR @ToAddressLongitude <> @OldToAddressLongitude) THEN
        DELETE FROM tbl_SpatialTrempRequest WHERE TrempRequestId = @OldTrempRequestId;
        IF (@FromAddressLatitude IS NOT NULL AND @FromAddressLongitude IS NOT NULL AND @ToAddressLatitude IS NOT NULL AND @ToAddressLongitude IS NOT NULL) THEN
            INSERT INTO tbl_SpatialTrempRequest (TrempRequestId, FromAddress, ToAddress) VALUES(@TrempRequestId,POINT(@FromAddressLatitude,@FromAddressLongitude),POINT(@ToAddressLatitude,@ToAddressLongitude));
	    END IF;
	END IF;
END 
$$

CREATE TRIGGER tg_tremprequest_Delete 
AFTER DELETE ON tbl_TrempRequest
FOR EACH ROW
BEGIN
	SET @OldTrempRequestId=OLD.TrempRequestId;
    DELETE FROM tbl_SpatialTrempRequest WHERE TrempRequestId = @OldTrempRequestId;
END
$$

DELIMITER ;";

                conn.ExecuteScript(dropTriggers);
                conn.ExecuteScript(createTriggers);

                conn.CommitTransaction();
            }
        }
    }
}
