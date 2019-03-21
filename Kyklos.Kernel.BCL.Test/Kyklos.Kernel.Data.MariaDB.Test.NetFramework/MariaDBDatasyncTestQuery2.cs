using System.Threading.Tasks;
using Kyklos.Kernel.Data.Test.Entities;
using Xunit;

namespace Kyklos.Kernel.Data.MariaDB.Test.NetFramework
{
    public class MariaDBDatasyncTestQuery2 : MariaDBDatasyncTestCommon
    {
        public MariaDBDatasyncTestQuery2()
        {            
        }

        #region Collection 2

        [Fact]
        public void GetResultTableMetadataFromQueryShouldBe()
        {
            string sql = "SELECT r.GOALS_HOME_TEAM, r.GOALS_VISITOR_TEAM FROM RESULTS r";

            GetResultTableMetadataFromQueryShouldBeCore(sql).Wait();
        }

        [Fact]
        public async Task FillDayDataTableShouldBe()
        {
            string sql = "SELECT d.* FROM DAYS d";

            await FillDayDataTableShouldBeCore(sql);
        }

        [Fact]
        public void GetScriptForCreateTableShouldBe()
        {
            string sql = "create table `testdb`.`TEAMS` (`TEAM_ID` NVARCHAR(50) not null, `NAME` NVARCHAR(200) not null, `CITY` NVARCHAR(150) not null, `PRESIDENT` NVARCHAR(120) not null, constraint TEAMS_pk primary key (`TEAM_ID`))";

            GetScriptForCreateTableShouldBeCore(sql, false, typeof(Team));
        }

        [Fact]
        public void GetScriptForDropTableShouldBe()
        {
            string sql = "drop table `testdb`.`RESULTS`";

            GetScriptForDropTableShouldBeCore(sql, "RESULTS");
        }

        [Fact]
        public void GetScriptsForForeignKeyShouldBe()
        {
            string[] sql = new string[]
            {
                "alter table `testdb`.`RESULTS` add constraint `HOME_TEAM_REF` foreign key (`HOME_TEAM_ID`) references `testdb`.`TEAMS`(`TEAM_ID`)",
                "alter table `testdb`.`RESULTS` add constraint `VIS_TEAM_REF` foreign key (`VISITOR_TEAM_ID`) references `testdb`.`TEAMS`(`TEAM_ID`)",
                "alter table `testdb`.`RESULTS` add constraint `DAY_REF` foreign key (`DAY_ID`) references `testdb`.`DAYS`(`DAY_ID`)"
            };

            GetScriptsForForeignKeyShouldBeCore(sql, typeof(Result));
        }

        [Fact]
        public void GetScriptForUniqueConstraintShouldBe()
        {
            string[] sql = new string[]
            {
                "create unique index UNIQUE_DAY on `testdb`.`DAYS` (`DAY_NUMBER`)"
            };

            GetScriptForUniqueConstraintShouldBeCore(sql, typeof(Day));
        }

        [Fact]
        public void GetScriptForCreateNonUniqueIndexShouldBe()
        {
            string[] sql = new string[]
            {
                "create index TEAM_INDEX on `testdb`.`TEAMS` (`NAME`, `CITY`)"
            };

            GetScriptForCreateNonUniqueIndexShouldBeCore(sql, typeof(Team));
        }

        [Fact]
        public void TestForCoalesce()
        {
            TestForCoalesceCore();
        }

        #endregion

    }
}
