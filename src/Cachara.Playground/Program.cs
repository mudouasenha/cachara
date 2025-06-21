using Cachara.Playground;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

MappingConfig.RegisterMappings();

var mapsterTest = new MapsterTests();

mapsterTest.TestMapster();

app.Run();
