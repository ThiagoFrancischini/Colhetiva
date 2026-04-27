using Colhetiva.Core.Entities;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Tools
{
    public static class SessionTools
    {
        private static readonly string KEY_USER = "USER_LOGADO";
        public static void SalvarUsuarioLogado(Usuario usuario)
        {
            if (usuario == null || usuario.Id == Guid.Empty) 
            {
                throw new ApplicationException("Informe um usuario valido!");
            }

            // Preferences.Set(KEY_USER, Newtonsoft.Json.JsonConvert.SerializeObject(usuario));
        }
        /*
        public static Usuario? GetUsuarioLogado()
        {
            if (!Preferences.ContainsKey(KEY_USER)) 
            {
                return null;
            }
          
            return JsonConvert.DeserializeObject<Usuario>(Preferences.Get(KEY_USER, string.Empty));
        } */

        public static void LimparSessao()
        {
            //Preferences.Remove(KEY_USER);
        }
    }
}
