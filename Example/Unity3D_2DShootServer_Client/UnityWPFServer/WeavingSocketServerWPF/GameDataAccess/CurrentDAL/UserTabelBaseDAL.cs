
using GameDataAccess.BaseDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDataAccess.CurrentDAL
{
    public  class UserTabelBaseDAL : Respository<UserTabel>
    {


        public new bool Insert(UserTabel newuser)
        {

            List<UserTabel> allItem = base.GetALLEntity().ToList();
            if (allItem.Count == 0)
            {
                return base.Insert(newuser);
            }
            else
            {
                //判定所有元素里面有没有这个元素。。。
                bool itemInList = allItem.Where(u => u.userName == newuser.userName).FirstOrDefault() != null ? true : false;
                if (itemInList)
                {
                    return false;
                }
                else
                {
                    return base.Insert(newuser);
                }

            }

        }



        public new bool Update(UserTabel m)
        {

            List<UserTabel> allItem = base.GetALLEntity().ToList();

            //判定所有元素里面有没有这个元素。。。
            bool itemInList = allItem.Where(dbm => dbm.userName == m.userName).FirstOrDefault() != null ? true : false;
            if (itemInList)
            {
                return false;
            }
            else
            {
                return base.Update(m);
            }



        }

    }
}
