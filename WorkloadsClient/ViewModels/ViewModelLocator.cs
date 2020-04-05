using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

namespace WorkloadsClient.ViewModels
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel
        {
            get
            {
                return GetMainViewModel().Result;
            }
        }

        public static async Task<MainViewModel> GetMainViewModel()
        {
            MainViewModel vm = App.ServiceProvider.GetRequiredService<MainViewModel>();

            await vm.GetAllAsync();

            return vm;
        }
    }


}
