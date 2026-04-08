using Colhetiva.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colhetiva.Core.Interfaces.Service
{
    public interface ICidadeService
    {
        public Task<List<Cidade>> ProcurarPorUF(Guid estadoId);

        public Task<Cidade> Salvar(Cidade cidade);
    }
}
