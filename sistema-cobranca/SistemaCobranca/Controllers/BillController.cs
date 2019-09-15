using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Http;

namespace SistemaCobranca.Controllers
{
    public class BillController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Post(string name, string cpf, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest();

            if (string.IsNullOrWhiteSpace(cpf))
                return BadRequest();

            if (string.IsNullOrWhiteSpace(value))
                return BadRequest();

            return Ok();
        }
    }
}
