using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToAzureCosmosDB.DTOs
{
    public class CreateTodo
    {
        public string id { get; set; } = String.Empty;

        public string Tenantid { get; set; } = String.Empty;

        public string Title { get; set; } = String.Empty;
        public bool IsCompleted { get; set; }
    }
}
