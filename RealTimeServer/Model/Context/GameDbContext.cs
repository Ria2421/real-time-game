using Microsoft.EntityFrameworkCore;
using RealTimeServer.Model.Entity;

namespace MagicOnionServer.Model.Context
{
    public class GameDbContext:DbContext
    {
        // テーブル(エンティティ) を追加したらここに追記していく
        public DbSet<User> Users { get; set; }

        // 接続先のDB (ローカル・オンラインで "server="の接続先が変わる)
        readonly string connectionString = "server=localhost;database=daydream_racing;user=jobi;password=jobi;";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // MySQLのバージョン指定
            optionsBuilder.UseMySql(connectionString,new MySqlServerVersion(new Version(8, 0)));
        }
    }
}
