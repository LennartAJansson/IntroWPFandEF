using Microsoft.Extensions.DependencyInjection;

namespace WorkloadsClient.ViewModels
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel => App.ServiceProvider.GetRequiredService<MainViewModel>();
    }
}
