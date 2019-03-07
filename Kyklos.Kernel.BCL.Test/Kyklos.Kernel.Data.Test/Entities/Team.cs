using System;
using System.Collections.Generic;
using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;

namespace Kyklos.Kernel.Data.Test.Entities
{
    [EntityObjectInfo(TableName = "TEAMS")]
    [EntityIndex(IndexName = "TEAM_INDEX", Properties = new string[] { nameof(Name), nameof(City) })]
    public class Team : IBaseEntity
    {
        [EntityPropertyInfo(ColumnName = "TEAM_ID", DbType = PropertyDbType.String, IsNullable = false, IsPrimaryKey = true, MaxLength = 50)]
        public string TeamId { get; set; }

        [EntityPropertyInfo(ColumnName = "NAME", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 200)]
        public string Name { get; set; }

        [EntityPropertyInfo(ColumnName = "CITY", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 150)]
        public string City { get; set; }

        [EntityPropertyInfo(ColumnName = "PRESIDENT", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 120)]
        public string President { get; set; }

        public override bool Equals(object obj)
        {
            var team = obj as Team;
            return team != null &&
                   TeamId == team.TeamId &&
                   Name == team.Name &&
                   City == team.City &&
                   President == team.President;
        }

        public override int GetHashCode()
        {
            var hashCode = -82492496;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TeamId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(City);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(President);
            return hashCode;
        }

        public static bool operator ==(Team team1, Team team2)
        {
            return EqualityComparer<Team>.Default.Equals(team1, team2);
        }

        public static bool operator !=(Team team1, Team team2)
        {
            return !(team1 == team2);
        }
    }
}
