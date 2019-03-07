using System;
using System.Collections.Generic;
using Kyklos.Kernel.Data.Attributes;
using Kyklos.Kernel.Data.Entities;


namespace Kyklos.Kernel.Data.Test.Entities
{
    [EntityObjectInfo(TableName = "RESULTS")]
    public class Result : IBaseEntity
    {
        [EntityPropertyInfo(ColumnName = "RESULT_ID", DbType = PropertyDbType.String, IsNullable = false, IsPrimaryKey = true, MaxLength = 50)]
        public string ResultId { get; set; }

        [EntityPropertyInfo(ColumnName = "HOME_TEAM_ID", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 50)]
        [EntityPropertyForeignKey(ConstraintName = "HOME_TEAM_REF", ReferencedEntity = typeof(Team), ReferencedPropertyName = nameof(Team.TeamId))]
        public string HomeTeamId { get; set; }

        [EntityPropertyInfo(ColumnName = "VISITOR_TEAM_ID", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 50)]
        [EntityPropertyForeignKey(ConstraintName = "VIS_TEAM_REF", ReferencedEntity = typeof(Team), ReferencedPropertyName = nameof(Team.TeamId))]
        public string VisitorTeamId { get; set; }

        [EntityPropertyInfo(ColumnName = "GOALS_HOME_TEAM", DbType = PropertyDbType.Integer, IsNullable = false)]
        public int GoalsHomeTeam { get; set; }

        [EntityPropertyInfo(ColumnName = "GOALS_VISITOR_TEAM", DbType = PropertyDbType.Integer, IsNullable = false)]
        public int GoalsVisitorTeam { get; set; }

        [EntityPropertyInfo(ColumnName = "DAY_ID", DbType = PropertyDbType.String, IsNullable = false, MaxLength = 50)]
        [EntityPropertyForeignKey(ConstraintName = "DAY_REF", ReferencedEntity = typeof(Day), ReferencedPropertyName = nameof(Day.DayId))]
        public string DayId { get; set; }

        public override bool Equals(object obj)
        {
            var result = obj as Result;
            return result != null &&
                   ResultId == result.ResultId &&
                   HomeTeamId == result.HomeTeamId &&
                   VisitorTeamId == result.VisitorTeamId &&
                   GoalsHomeTeam == result.GoalsHomeTeam &&
                   GoalsVisitorTeam == result.GoalsVisitorTeam &&
                   DayId == result.DayId;
        }

        public override int GetHashCode()
        {
            var hashCode = 359941307;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ResultId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HomeTeamId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(VisitorTeamId);
            hashCode = hashCode * -1521134295 + GoalsHomeTeam.GetHashCode();
            hashCode = hashCode * -1521134295 + GoalsVisitorTeam.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DayId);
            return hashCode;
        }

        public static bool operator ==(Result result1, Result result2)
        {
            return EqualityComparer<Result>.Default.Equals(result1, result2);
        }

        public static bool operator !=(Result result1, Result result2)
        {
            return !(result1 == result2);
        }
    }
}
