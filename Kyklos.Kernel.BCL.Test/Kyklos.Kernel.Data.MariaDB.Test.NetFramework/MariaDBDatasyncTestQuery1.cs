using Xunit;

namespace Kyklos.Kernel.Data.MariaDB.Test.NetFramework
{
    public class MariaDBDatasyncTestQuery1 : MariaDBDatasyncTestCommon
    {
        public MariaDBDatasyncTestQuery1()
        {            
        }

        #region Collection 1

        [Fact]
        public void CheckIfDbSupportsValuesForFastInConditionShouldBe()
        {
            CheckIfDbSupportsValuesForFastInConditionShouldBeCore(true);
        }

        [Fact]
        public void CheckDbProviderNameShouldBe()
        {
            CheckDbProviderNameShouldBeCore("MariaDB");
        }

        [Fact]
        public void SqlInnerJoinShouldBe()
        {
            string sqlJoin = "testdb.RESULTS r inner join testdb.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " inner join testdb.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " inner join testdb.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlInnerJoinShouldBeCore(sqlJoin);
        }


        [Fact]
        public void SqlLeftJoinShouldBe()
        {
            string sqlJoin = "testdb.RESULTS r left join testdb.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " left join testdb.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " left join testdb.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlLeftJoinShouldBeCore(sqlJoin);
        }

        [Fact]
        public void SqlRightJoinShouldBe()
        {
            string sqlJoin = "testdb.RESULTS r right join testdb.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " right join testdb.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " right join testdb.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlRightJoinShouldBeCore(sqlJoin);
        }

        [Fact]
        public void SqlFullOuterJoinShouldBe()
        {
            string sqlJoin = "testdb.RESULTS r full outer join testdb.DAYS d on (r.DAY_ID = d.DAY_ID)" +
                              " full outer join testdb.TEAMS ht on (r.HOME_TEAM_ID = ht.TEAM_ID)" +
                              " full outer join testdb.TEAMS vt on (r.VISITOR_TEAM_ID = vt.TEAM_ID)";

            SqlFullOuterJoinShouldBeCore(sqlJoin);
        }

        [Fact]
        public void SqlInnerJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = "`testdb`.`RESULTS` r inner join `testdb`.`DAYS` d on (r.`DAY_ID` = d.`DAY_ID`)" +
                              " inner join `testdb`.`TEAMS` ht on (r.`HOME_TEAM_ID` = ht.`TEAM_ID`)" +
                              " inner join `testdb`.`TEAMS` vt on (r.`VISITOR_TEAM_ID` = vt.`TEAM_ID`)";

            SqlInnerJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlLeftJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = "`testdb`.`RESULTS` r left join `testdb`.`DAYS` d on (r.`DAY_ID` = d.`DAY_ID`)" +
                              " left join `testdb`.`TEAMS` ht on (r.`HOME_TEAM_ID` = ht.`TEAM_ID`)" +
                              " left join `testdb`.`TEAMS` vt on (r.`VISITOR_TEAM_ID` = vt.`TEAM_ID`)";

            SqlLeftJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlRightJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = "`testdb`.`RESULTS` r right join `testdb`.`DAYS` d on (r.`DAY_ID` = d.`DAY_ID`)" +
                              " right join `testdb`.`TEAMS` ht on (r.`HOME_TEAM_ID` = ht.`TEAM_ID`)" +
                              " right join `testdb`.`TEAMS` vt on (r.`VISITOR_TEAM_ID` = vt.`TEAM_ID`)";

            SqlRightJoinShouldBeCore(sqlJoin, false);
        }


        [Fact]
        public void SqlFullOuterJoinWithEscapeShouldBeCore()
        {
            string sqlJoin = "`testdb`.`RESULTS` r full outer join `testdb`.`DAYS` d on (r.`DAY_ID` = d.`DAY_ID`)" +
                              " full outer join `testdb`.`TEAMS` ht on (r.`HOME_TEAM_ID` = ht.`TEAM_ID`)" +
                              " full outer join `testdb`.`TEAMS` vt on (r.`VISITOR_TEAM_ID` = vt.`TEAM_ID`)";

            SqlFullOuterJoinShouldBeCore(sqlJoin, false);
        }

        [Fact]
        public void TestForNullShouldProduceIsNullOperator()
        {
            TestForNullShouldProduceIsNullOperatorCore();
        }

        #endregion

    }
}
