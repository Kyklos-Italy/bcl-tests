using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Film
{
    public class JoinCastFilm : IBaseEntity
    {
        [EntityPropertyInfo(ColumnName = "CAST.ID", DbType = PropertyDbType.Integer, IsNullable = false, IsPrimaryKey = true)]
        public int Id { get; set; }

        [EntityPropertyInfo(ColumnName = "FILMID", DbType = PropertyDbType.Integer, IsNullable = false)]
        [EntityPropertyForeignKey(ConstraintName = "ID", ReferencedEntity = typeof(Film), ReferencedPropertyName = nameof(Film.Id))]
        public int FilmId { get; set; }

        [EntityPropertyInfo(ColumnName = "ACTORID", DbType = PropertyDbType.Integer, IsNullable = false)]
        [EntityPropertyForeignKey(ConstraintName = "ID", ReferencedEntity = typeof(Actor), ReferencedPropertyName = nameof(Actor.Id))]
        public int ActorId { get; set; }

        [EntityPropertyInfo(ColumnName = "ACTORROLE", DefaultValue = "", DbType = PropertyDbType.String, MaxLength = 20, IsNullable = false)]
        public string Role { get; set; }

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
