////////////////////////////////////////////////////////////////
///
/// データベースとの接続を管理するスクリプト
/// 
/// Aughter:木田晃輔
///
////////////////////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace WIA.Server.Model.Context
{
    /// <summary>
    /// データベースの設定、SQLとの接続を行う
    /// </summary>
    public class GameDbContext : DbContext
    {


#if DEBUG
        //server名;ユーザー名;パスワード指定
        readonly string connectionString = "server=localhost;database=admin_console;user=jobi;password=jobi;";
#else
        readonly string connectionString = "server=db-ge-07.mysql.database.azure.com;database=nightraveldb;user=student;password=Yoshidajobi2023;";
#endif

        //SQLと接続
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString,
                                                new MySqlServerVersion(new Version(8, 0)));
        }
    }
}
