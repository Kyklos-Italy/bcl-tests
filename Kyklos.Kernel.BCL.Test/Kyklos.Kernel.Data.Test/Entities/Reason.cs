using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;
using System.Collections.Generic;

namespace Kyklos.Kernel.Data.Test.Entities
{
    [EntityObjectInfo(TableName = "REASONS")]
    public class Reason : IBaseEntity
    {
        [EntityPropertyInfo(ColumnName = "ReasonCode", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 3, ColumnOrder = 10)]
        public virtual string ReasonCode { get; set; }

        [EntityPropertyInfo(ColumnName = "ReasonDesc", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 50, ColumnOrder = 20)]
        public virtual string ReasonDesc { get; set; }

        [EntityPropertyInfo(ColumnName = "ReasonID", DbType = PropertyDbType.Integer, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 30)]
        public virtual int ReasonId { get; set; }

        [EntityPropertyInfo(ColumnName = "ReconcileValuationTypeID", DbType = PropertyDbType.Integer, IsNullable = true, ColumnOrder = 40)]
        public virtual int? ReconcileValuationTypeId { get; set; }

    }
}