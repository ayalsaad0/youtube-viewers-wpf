using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeViewers.Domain.Models;
using YouTubeViewers.WPF.Stores;
using YouTubeViewers.WPF.ViewModels;

namespace YouTubeViewers.WPF.Commands
{
    public class AddYouTubeViewerCommand : AsyncCommandBase
    {
        private readonly AddYouTubeViewerViewModel _addYouTubeViewerViewModel;
        private readonly YouTubeViewersStore _youTubeViewersStore;
        private readonly ModalNavigationStore _modalNavigateStore;

        public AddYouTubeViewerCommand(AddYouTubeViewerViewModel addYouTubeViewerViewModel, YouTubeViewersStore youTubeViewersStore, ModalNavigationStore modalNavigationStore)
        {
            _addYouTubeViewerViewModel = addYouTubeViewerViewModel;
            _youTubeViewersStore = youTubeViewersStore;
            _modalNavigateStore = modalNavigationStore;
        }
        public override async Task ExecuteAsync(object parameter)
        {
            YouTubeViewerDetailsFormViewModel formViewModel = _addYouTubeViewerViewModel.YouTubeViewerDetailsFormViewModel;

            formViewModel.ErrorMessage = null;
            formViewModel.IsSubmitting = true;

            YouTubeViewer youTubeViewer = new YouTubeViewer(Guid.NewGuid(), formViewModel.Username, formViewModel.IsSubscribed, formViewModel.IsMember);

            try
            {
                await _youTubeViewersStore.Add(youTubeViewer);

                _modalNavigateStore.Close();
            }
            catch (Exception)
            {
                formViewModel.ErrorMessage = "Failed to add the YouTube viewer. Please try again.";
            }
            finally
            {
                formViewModel.IsSubmitting = false;
            }

        }
    }
}
