using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sapataria_Almeida.Commands;       // RelayCommand
using Sapataria_Almeida.Repositories;   // IRepositorioDados

namespace Sapataria_Almeida.ViewModels
{
    public class NotificacaoViewModel : INotifyPropertyChanged
    {
        private readonly IRepositorioDados _repositorio;
        public int Id { get; }
        public string Mensagem { get; }

        public ICommand FecharCommand { get; }

        public NotificacaoViewModel(int id, string mensagem, IRepositorioDados repositorio)
        {
            Id = id;
            Mensagem = mensagem;
            _repositorio = repositorio;
            FecharCommand = new RelayCommand(_ => Fechar());
        }

        private async void Fechar()
        {
            // marca como lida no banco
            await _repositorio.MarcarNotificacaoComoLidaAsync(Id);
            // remove da coleção do parent (implementado lá)
            OnNotificacaoFechada?.Invoke(this, EventArgs.Empty);
        }

        // evento opcional para avisar o MainPageViewModel
        public event EventHandler? OnNotificacaoFechada;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
