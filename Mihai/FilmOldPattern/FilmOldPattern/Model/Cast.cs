using Kyklos.Kernel.BE.Support.Entities;
using Kyklos.Kernel.SpringSupport.Data.Attributes;
using Kyklos.Kernel.SpringSupport.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmOldPattern.Model
{
    [EntityObjectInfo(TableName = "CAST")]
    [EntityUniqueConstraint(ConstraintName = "UNIQUE_ACTORFILMROLE", Properties = new string[] { nameof(FilmId), nameof(ActorId), nameof(Role) })]
    public class Cast : BaseEntityWithLongKey<Cast>
    {
        public Cast()
            : base(x => x.Id)
        {
        }
        [EntityPropertyInfo(ColumnName = "ID", DbType = PropertyDbType.Integer, IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }

        [EntityPropertyInfo(ColumnName = "FILMID", DbType = PropertyDbType.Integer, IsNullable = false)]
        [EntityPropertyForeignKey(ConstraintName = "ID", ReferencedEntity = typeof(Film), ReferencedPropertyName = nameof(Film.Id))]
        public int FilmId { get; set; }

        [EntityPropertyInfo(ColumnName = "ACTORID", DbType = PropertyDbType.Integer, IsNullable = false)]
        [EntityPropertyForeignKey(ConstraintName = "ID", ReferencedEntity = typeof(Actor), ReferencedPropertyName = nameof(Actor.Id))]
        public int ActorId { get; set; }

        [EntityPropertyInfo(ColumnName = "ACTORROLE", DefaultValue = "", DbType = PropertyDbType.String, MaxLength = 20, IsNullable = false)]
        public string Role { get; set; }
    }
}
