using Microsoft.EntityFrameworkCore;
using RealTimeServer.Model.Entity;
using Shared.Model.Entity;

namespace MagicOnionServer.Model.Context
{
    public class GameDbContext:DbContext
    {
        // テーブル(エンティティ) を追加したらここに追記していく
        public DbSet<User> Users { get; set; }
        public DbSet<SoloPlayData> Solo_Play_Data { get; set; }
        public DbSet<SoloPlayLog> Solo_Play_Logs { get; set; }
        public DbSet<NGWord> Ng_Words { get; set; }

        // 接続先のDB (ローカル・オンラインで "server="の接続先が変わる)
#if DEBUG
        readonly string connectionString = "server=localhost;database=carboomcrash;user=jobi;password=jobi;";
#else
        readonly string connectionString = "server=db-ge-07.mysql.database.azure.com;database=carboomcrash;user=student;password=Yoshidajobi2023;";
#endif
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // MySQLのバージョン指定
            optionsBuilder.UseMySql(connectionString,new MySqlServerVersion(new Version(8, 0)));
        }
    }
}
