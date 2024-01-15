using Dapper;
using Microsoft.Data.Sqlite;

using var cnn = new SqliteConnection($"Data Source=sample.sqlite");
cnn.Open();
try
{
    cnn.Execute(
        @"create table Say
        (
            Id      integer primary key AUTOINCREMENT,
            Hello   varchar(100) not null,
            At      datetime not null
        )");
}
catch (SqliteException ex) when (ex is { SqliteErrorCode: 1 }) //table Say already exists
{
    Console.Write(ex.Message);
}
var i = cnn.Query<Say>(@"INSERT INTO Say ( Hello, At ) VALUES ( @Hello, @At ) RETURNING *", new Say { Hello = "World", At = DateTime.Now }).Single();

var builder = WebApplication.CreateSlimBuilder(args);
var app = builder.Build();
app.MapGet("i/said", () => i);
app.MapGet("{id}/said", (int id) => cnn.Query<Say>(@"SELECT * FROM Say WHERE Id = @id", new { id }).Single());
app.MapGet("says", () => cnn.Query<Say>(@"SELECT * FROM Say"));
app.Run();

class Say
{
    public int? Id { get; set; }
    public string Hello { get; set; }
    public DateTime At { get; set; }
}