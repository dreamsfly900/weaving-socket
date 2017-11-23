using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDataAccess.CurrentDAL;
namespace GameDataAccess.BLL
{
     public class UserTableBLL
    {

        public bool CheckUserNamePassword(string uname,string pwd)
        {
            bool canLogin = false;

            UserTabelBaseDAL uDAL = new UserTabelBaseDAL();

            canLogin = uDAL.GetOneByWhere(u=>u.userName == uname && u.password == pwd) !=null ? true : false;

            return canLogin;
        }



        public bool CheckDataBaseIsNull()
        {
            bool databaseIsNull = false;

            UserTabelBaseDAL uDAL = new UserTabelBaseDAL();

            databaseIsNull = uDAL.GetALLEntity().Count == 0 ? true : false;

            return databaseIsNull;
        }

        public void AddTestData()
        {

            string new_userId = Guid.NewGuid().ToString("N");
            Model.UserTabel u = new Model.UserTabel()
            {
                Id = new_userId,
                userName = "linmeng",
                password = "111111",
                logintime = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            };

            Model.GameScoreTabel s = new Model.GameScoreTabel()
            {
                Id = Guid.NewGuid().ToString("N"),
                userId = new_userId,
                missenemy = 0,
                score = 888
            };






            UserTabelBaseDAL uDAL = new UserTabelBaseDAL();

            bool insertNewUser = uDAL.Insert(u);

            GameScoreTabelBaseDAL sDAL = new GameScoreTabelBaseDAL();

            bool insertNewScore = sDAL.Insert(s);


            string new_userId2 = Guid.NewGuid().ToString("N");
            Model.UserTabel u2 = new Model.UserTabel()
            {
                Id = new_userId2,
                userName = "admin",
                password = "111111",
                logintime = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            };

            Model.GameScoreTabel s2 = new Model.GameScoreTabel()
            {
                Id = Guid.NewGuid().ToString("N"),
                userId = new_userId2,
                missenemy = 0,
                score = 999
            };
             insertNewUser = uDAL.Insert(u2);

          

             insertNewScore = sDAL.Insert(s2);


            string new_userId3 = Guid.NewGuid().ToString("N");
            Model.UserTabel u3 = new Model.UserTabel()
            {
                Id = new_userId3,
                userName = "goodjob",
                password = "111111",
                logintime = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            };

            Model.GameScoreTabel s3 = new Model.GameScoreTabel()
            {
                Id = Guid.NewGuid().ToString("N"),
                userId = new_userId3,
                missenemy = 0,
                score = 555
            };

            insertNewUser = uDAL.Insert(u3);



            insertNewScore = sDAL.Insert(s3);


        }





    }
}
