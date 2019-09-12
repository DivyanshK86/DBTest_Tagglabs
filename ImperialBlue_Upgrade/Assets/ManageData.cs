using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageData : MonoBehaviour {


	void Start ()
    {
        CreateTable();
	}

    #region DATABASE
    public SimpleSQL.SimpleSQLManager dbManager;
    int UserId = 0;

    public void CreateTable()
    {
        dbManager.CreateTable<UserDetails>();
    }


    public void InsertData(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8)
    {
        getUserId();
        string dt = System.DateTime.Now.ToString();
        // set up our insert SQL statement with ? parameters
        string sql = "INSERT INTO UserDetails (userid, q1, q2, q3, q4, q5, q6, q7, q8, q9 ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

        dbManager.BeginTransaction();

        dbManager.Execute(sql, UserId, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, dt);

        dbManager.Commit();
    }

    public void getUserId()
    {
        string sql = "SELECT * FROM UserDetails";
        List<UserDetails> ud = dbManager.Query<UserDetails>(sql);
        if (ud.Count > 0)
        {
            UserId = (ud[(ud.Count - 1)].userid + 1);
        }
        else
        {
            UserId = 0;
        }

    }
    #endregion
}
