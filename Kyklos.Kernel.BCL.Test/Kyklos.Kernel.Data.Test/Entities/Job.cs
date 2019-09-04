using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;
using System.Collections.Generic;
namespace Kyklos.Kernel.Data.Test.Entities
{
    [EntityObjectInfo(TableName = "JOBS")]
    public class Job : IBaseEntity
    {
        [EntityPropertyInfo(ColumnName = "JobID", DbType = PropertyDbType.Long, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 10)]
        public virtual long JobId { get; set; }

        [EntityPropertyInfo(ColumnName = "JobName", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 65, ColumnOrder = 20)]
        public virtual string JobName { get; set; }

        [EntityPropertyInfo(ColumnName = "ShortDesc", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 50, ColumnOrder = 30)]
        public virtual string ShortDesc { get; set; }

        [EntityPropertyInfo(ColumnName = "LongDesc", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 8000, ColumnOrder = 40)]
        public virtual string LongDesc { get; set; }

        [EntityPropertyInfo(ColumnName = "IsJobActive", DbType = PropertyDbType.Boolean, IsNullable = false, ColumnOrder = 50)]
        public virtual bool IsJobActive { get; set; }

        [EntityPropertyInfo(ColumnName = "CustomerID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 60)]
        public virtual long CustomerId { get; set; }

        [EntityPropertyInfo(ColumnName = "StateID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 70)]
        public virtual long StateId { get; set; }

        [EntityPropertyInfo(ColumnName = "DivisionID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 80)]
        public virtual long DivisionId { get; set; }

        [EntityPropertyInfo(ColumnName = "CategoryID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 90)]
        public virtual long CategoryId { get; set; }

        [EntityPropertyInfo(ColumnName = "OwnerID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 100)]
        public virtual long OwnerId { get; set; }

        [EntityPropertyInfo(ColumnName = "ExpectedReleaseDate", DbType = PropertyDbType.DateTime, IsNullable = true, ColumnOrder = 110)]
        public virtual System.DateTime ExpectedReleaseDate { get; set; }

        [EntityPropertyInfo(ColumnName = "ConfirmedReleaseDate", DbType = PropertyDbType.DateTime, IsNullable = true, ColumnOrder = 120)]
        public virtual System.DateTime ConfirmedReleaseDate { get; set; }

        [EntityPropertyInfo(ColumnName = "JobNote", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 8000, ColumnOrder = 130)]
        public virtual string JobNote { get; set; }

        [EntityPropertyInfo(ColumnName = "CustomerOrderRef", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 100, ColumnOrder = 140)]
        public virtual string CustomerOrderRef { get; set; }

        [EntityPropertyInfo(ColumnName = "InternalOfferCode", DbType = PropertyDbType.String, IsNullable = true, MaxLength = 50, ColumnOrder = 150)]
        public virtual string InternalOfferCode { get; set; }

        [EntityPropertyInfo(ColumnName = "IDFather", DbType = PropertyDbType.Long, IsNullable = true, ColumnOrder = 160)]
        public virtual long? IdFather { get; set; }

        [EntityPropertyInfo(ColumnName = "ProductId", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 170)]
        public virtual long ProductId { get; set; }

        [EntityPropertyInfo(ColumnName = "BusinessTeamId", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 180)]
        public virtual long BusinessTeamId { get; set; }

        [EntityPropertyInfo(ColumnName = "InvCompanyID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 190)]
        public virtual long InvCompanyId { get; set; }

        [EntityPropertyInfo(ColumnName = "TipologyID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 200)]
        public virtual long TipologyId { get; set; }

        [EntityPropertyInfo(ColumnName = "SubTipologyID", DbType = PropertyDbType.Long, IsNullable = false, ColumnOrder = 210)]
        public virtual long SubTipologyId { get; set; }

        [EntityPropertyInfo(ColumnName = "ProgressPerc", DbType = PropertyDbType.Integer, IsNullable = true, ColumnOrder = 220)]
        public virtual int? ProgressPerc { get; set; }

    }
}