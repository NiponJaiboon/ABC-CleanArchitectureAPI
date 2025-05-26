using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Remark { get; set; }
    }
}
// dotnet ef migrations add AddRemark --project Infrastructure/Infrastructure.csproj --startup-project API/API.csproj --context FirstDbContext
// dotnet ef migrations add AddRemark --context FirstDbContext --startup-project ../API/ABC.API.csproj
// dotnet ef database update  --context FirstDbContext --startup-project ../API/ABC.API.csproj