using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;
using System;
using System.Collections.Generic;

namespace Kyklos.Kernel.Data.Test.Entities
{
    [EntityObjectInfo(TableName = "JOB_TIME_OF_THE_DAY")]
    public class JobTimesOfTheDay : IBaseEntity
    {
        [EntityPropertyInfo(ColumnName = "hours", DbType = PropertyDbType.Decimal, IsNullable = true, ColumnOrder = 10)]
        public virtual decimal? Hours { get; set; }

        [EntityPropertyInfo(ColumnName = "reason", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 50, ColumnOrder = 20)]
        public virtual string Reason { get; set; }

        [EntityPropertyInfo(ColumnName = "job", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 65, ColumnOrder = 30)]
        public virtual string Job { get; set; }

        [EntityPropertyInfo(ColumnName = "dateOfWork", DbType = PropertyDbType.DateTime, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 40)]
        public virtual DateTime Dateofwork { get; set; }

        [EntityPropertyInfo(ColumnName = "JobId", DbType = PropertyDbType.Long, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 50)]
        public virtual long JobId { get; set; }

        [EntityPropertyInfo(ColumnName = "ReasonID", DbType = PropertyDbType.Integer, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 60)]
        public virtual int ReasonId { get; set; }

        [EntityPropertyInfo(ColumnName = "MemberId", DbType = PropertyDbType.Long, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 70)]
        public virtual long MemberId { get; set; }

        [EntityPropertyInfo(ColumnName = "DateWork", DbType = PropertyDbType.DateTime, IsNullable = false, IsPrimaryKey = true, ColumnOrder = 80)]
        public virtual DateTime DateWork { get; set; }
     }

}
