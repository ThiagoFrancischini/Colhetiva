using Colhetiva.Core.Entities;
using Colhetiva.Core.Input;
using Colhetiva.Core.Interfaces.Repositories;
using Colhetiva.Core.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colhetiva.Core.Services
{
    public class HortaService : IHortaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextRepository _userContextRepository;

        public HortaService(IUnitOfWork unitOfWork, IUserContextRepository userContextRepository)
        {
            _unitOfWork = unitOfWork;
            _userContextRepository = userContextRepository;
        }

        public async Task<List<Horta>> GetAllAsync()
        {
            return await _unitOfWork.HortaRepository.GetHortas();
        }

        public async Task<Horta?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.HortaRepository.GetById(id);
        }

        public async Task<Horta> CriarHortaAsync(Horta novaHorta)
        {
            novaHorta.Id = Guid.NewGuid();
            await _unitOfWork.HortaRepository.Salvar(novaHorta);
            await _unitOfWork.CompleteAsync();
            return novaHorta;
        }

        public async Task<Horta> CriarCompletoAsync(HortaCadastroInput input)
        {
            var enderecoId = Guid.NewGuid();

            var endereco = new Endereco
            {
                Id = enderecoId,
                Cep = input.Endereco.Cep,
                Rua = input.Endereco.Rua,
                Numero = input.Endereco.Numero,
                Bairro = input.Endereco.Bairro,
                Complemento = input.Endereco.Complemento,
                Latitude = input.Endereco.Latitude,
                Longitude = input.Endereco.Longitude,
                CidadeId = input.Endereco.CidadeId
            };


            await _unitOfWork.EnderecoRepository.Salvar(endereco);

            var hortaId = Guid.NewGuid();
            var horta = new Horta
            {
                Id = hortaId,
                Nome = input.Nome,
                Regras = input.Regras ?? string.Empty,
                EnderecoId = enderecoId,
                UsuarioId = input.UsuarioId
            };

            await _unitOfWork.HortaRepository.Salvar(horta);

            foreach (var linha in input.Canteiros.Where(c => !c.Remover && !string.IsNullOrWhiteSpace(c.Identificacao)))
            {
                var c = new Canteiro
                {
                    Id = Guid.NewGuid(),
                    Identificacao = linha.Identificacao.Trim(),
                    Dimensoes = linha.Dimensoes?.Trim() ?? string.Empty,
                    HortaId = hortaId,
                    Status = linha.Status
                };
                await _unitOfWork.CanteiroRepository.Salvar(c);
            }

            foreach (var linha in input.Ferramentas.Where(f => !f.Remover && !string.IsNullOrWhiteSpace(f.Nome)))
            {
                var f = new Ferramenta
                {
                    Id = Guid.NewGuid(),
                    Nome = linha.Nome.Trim(),
                    HortaId = hortaId,
                    Status = linha.Status
                };
                await _unitOfWork.FerramentaRepository.Salvar(f);
            }

            await _unitOfWork.CompleteAsync();
            return horta;
        }

        public async Task AtualizarCompletoAsync(HortaCadastroInput input)
        {
            var existente = await _unitOfWork.HortaRepository.GetById(input.Id);
            if (existente == null)
            {
                throw new InvalidOperationException("Horta não encontrada.");
            }

            var enderecoId = existente.EnderecoId;
            var endereco = new Endereco
            {
                Id = enderecoId,
                Cep = input.Endereco.Cep,
                Rua = input.Endereco.Rua,
                Numero = input.Endereco.Numero,
                Bairro = input.Endereco.Bairro,
                Complemento = input.Endereco.Complemento,
                Latitude = input.Endereco.Latitude,
                Longitude = input.Endereco.Longitude,
                CidadeId = input.Endereco.CidadeId
            };
            await _unitOfWork.EnderecoRepository.Salvar(endereco);

            var horta = new Horta
            {
                Id = input.Id,
                Nome = input.Nome,
                Regras = input.Regras ?? string.Empty,
                EnderecoId = enderecoId,
                UsuarioId = input.UsuarioId
            };
            await _unitOfWork.HortaRepository.Salvar(horta);

            foreach (var linha in input.Canteiros.Where(c => c.Remover && c.Id != Guid.Empty))
            {
                await _unitOfWork.CanteiroRepository.Excluir(linha.Id);
            }

            foreach (var linha in input.Canteiros.Where(c => !c.Remover && !string.IsNullOrWhiteSpace(c.Identificacao)))
            {
                if (linha.Id == Guid.Empty)
                {
                    var c = new Canteiro
                    {
                        Id = Guid.NewGuid(),
                        Identificacao = linha.Identificacao.Trim(),
                        Dimensoes = linha.Dimensoes?.Trim() ?? string.Empty,
                        HortaId = input.Id,
                        Status = linha.Status
                    };
                    await _unitOfWork.CanteiroRepository.Salvar(c);
                }
                else
                {
                    var c = new Canteiro
                    {
                        Id = linha.Id,
                        Identificacao = linha.Identificacao.Trim(),
                        Dimensoes = linha.Dimensoes?.Trim() ?? string.Empty,
                        HortaId = input.Id,
                        Status = linha.Status
                    };
                    await _unitOfWork.CanteiroRepository.Salvar(c);
                }
            }

            foreach (var linha in input.Ferramentas.Where(f => f.Remover && f.Id != Guid.Empty))
            {
                await _unitOfWork.FerramentaRepository.Excluir(linha.Id);
            }

            foreach (var linha in input.Ferramentas.Where(f => !f.Remover && !string.IsNullOrWhiteSpace(f.Nome)))
            {
                if (linha.Id == Guid.Empty)
                {
                    var f = new Ferramenta
                    {
                        Id = Guid.NewGuid(),
                        Nome = linha.Nome.Trim(),
                        HortaId = input.Id,
                        Status = linha.Status
                    };
                    await _unitOfWork.FerramentaRepository.Salvar(f);
                }
                else
                {
                    var f = new Ferramenta
                    {
                        Id = linha.Id,
                        Nome = linha.Nome.Trim(),
                        HortaId = input.Id,
                        Status = linha.Status
                    };
                    await _unitOfWork.FerramentaRepository.Salvar(f);
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task ExcluirAsync(Guid id)
        {
            var horta = await _unitOfWork.HortaRepository.GetById(id);
            if (horta == null)
            {
                return;
            }

            var enderecoId = horta.EnderecoId;
            await _userContextRepository.DesvincularHortaAsync(id);
            await _unitOfWork.HortaRepository.Excluir(id);
            await _unitOfWork.EnderecoRepository.Excluir(enderecoId);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<Horta>> FiltrarAsync(string? nome, string? cidade)
        {
            var hortas = await _unitOfWork.HortaRepository.GetHortas();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                hortas = hortas
                    .Where(h => h.Nome != null && h.Nome.ToLower().Contains(nome.ToLower()))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(cidade))
            {
                hortas = hortas
                    .Where(h => h.Endereco != null
                             && h.Endereco.Cidade != null
                             && h.Endereco.Cidade.Nome.ToLower().Contains(cidade.ToLower()))
                    .ToList();
            }

            return hortas;
        }
    }
}
