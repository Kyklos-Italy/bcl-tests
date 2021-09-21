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
    [EntityObjectInfo(TableName = "FILMS")]
    public class Film : BaseEntityWithLongKey<Film>
    {
        public Film()
            : base(x => x.Id)
        {
        }

        [EntityPropertyInfo(ColumnName = "ID", DbType = PropertyDbType.Long, IsNullable = false, IsPrimaryKey = true)]
        public long Id { get; set; }

        [EntityPropertyInfo(ColumnName = "FILMTITLE", DbType = PropertyDbType.String, MaxLength = 50, IsNullable = false)]
        public string FilmTitle { get; set; }

        [EntityPropertyInfo(ColumnName = "FILMKIND", DbType = PropertyDbType.String, MaxLength = 10, IsNullable = false)]
        public string FilmKind { get; set; }

        [EntityPropertyInfo(ColumnName = "FILMPLOT", DbType = PropertyDbType.String, MaxLength = 100, IsNullable = false)]
        public string FilmPlot { get; set; }

        [EntityPropertyInfo(ColumnName = "LANGUAGE", DbType = PropertyDbType.String, MaxLength = 10, IsNullable = false)]
        public string Language { get; set; }

        [EntityPropertyInfo(ColumnName = "DATE", DbType = PropertyDbType.DateTime, IsNullable = false)]
        public DateTime Date { get; set; }
    }
}
