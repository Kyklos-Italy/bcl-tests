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
    [EntityObjectInfo(TableName = "ACTORS")]
    [EntityUniqueConstraint(ConstraintName = "UNIQUE_TAXIDCODE", Properties = new string[] { nameof(ActorTaxIdCode) })]
    public class Actor : BaseEntityWithLongKey<Actor>
    {
        public Actor()
            : base(x => x.Id)
        {
        }
        [EntityPropertyInfo(ColumnName = "ID", DbType = PropertyDbType.Integer, IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }

        [EntityPropertyInfo(ColumnName = "ACTORNAME", DbType = PropertyDbType.String, MaxLength = 20, IsNullable = false)]
        public string ActorName { get; set; }

        [EntityPropertyInfo(ColumnName = "ACTORLASTNAME", DbType = PropertyDbType.String, MaxLength = 15, IsNullable = false)]
        public string ActorLastName { get; set; }

        [EntityPropertyInfo(ColumnName = "ACTORBIBLIOGRAPHY", DbType = PropertyDbType.String, MaxLength = 50, IsNullable = false)]
        public string ActorBibliography { get; set; }

        [EntityPropertyInfo(ColumnName = "ACTORTAXIDCODE", DbType = PropertyDbType.String, MaxLength = 50, IsNullable = false)]
        public string ActorTaxIdCode { get; set; }

        //[EntityPropertyInfo(ColumnName = "ACTORDATEBIRTH", DbType = PropertyDbType.DateTime, IsNullable = false)]
        //public DateTime ActorDateBirth { get; set; }

        [EntityPropertyInfo(ColumnName = "ACTORYEAR", DbType = PropertyDbType.Integer, IsNullable = false)]
        public int ActorYear { get; set; }


        public string GetInfo()
        {
            string info = $"[Id:{this.Id},ActorName:{this.ActorName},ActorLastName:{this.ActorLastName},ActorBibliography:{this.ActorBibliography},ActorTaxIdCode:{this.ActorTaxIdCode},Year:{this.ActorYear}";
            return info;
        }
    }
}
