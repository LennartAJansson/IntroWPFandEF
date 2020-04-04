using System.Collections.Generic;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Microsoft.Extensions.Logging;

using WorkloadsDb.Abstract;
using WorkloadsDb.Model;


namespace WorkloadsClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ILogger<MainViewModel> logger;
        private readonly IWorkloadService workloadsService;

        //The selected values:
        private Person person;
        //When a person is selected then filter Workloads on that person
        public Person SelectedPerson
        {
            get => person;
            set
            {
                Set(ref person, value);
                SetWorkloads(SelectedPerson == null ? 0 : SelectedPerson.PersonId, SelectedAssignment == null ? 0 : SelectedAssignment.AssignmentId);
            }
        }

        private Assignment assignment;
        //When an assignment is selected then filter workloads on that assignment
        public Assignment SelectedAssignment
        {
            get => assignment;
            set
            {
                Set(ref assignment, value);
                SetWorkloads(SelectedPerson == null ? 0 : SelectedPerson.PersonId, SelectedAssignment == null ? 0 : SelectedAssignment.AssignmentId);
            }
        }

        private Workload workload;
        public Workload SelectedWorkload { get => workload; set => Set(ref workload, value); }

        //The list of values:
        private IEnumerable<Person> people;
        public IEnumerable<Person> People
        {
            get => people;
            set => Set(ref people, value);
        }

        private IEnumerable<Assignment> assignments;
        public IEnumerable<Assignment> Assignments
        {
            get => assignments;
            set => Set(ref assignments, value);
        }

        private IEnumerable<Workload> workloads;
        public IEnumerable<Workload> Workloads
        {
            get => workloads;
            set => Set(ref workloads, value);
        }


        public RelayCommand<string> ExecuteCommand { get; }

        public MainViewModel(ILogger<MainViewModel> logger, IWorkloadService workloadsService)
        {
            this.logger = logger;
            this.workloadsService = workloadsService;
            ExecuteCommand = new RelayCommand<string>(async (string parameter) => await ExecuteAsync(parameter));
        }

        private async Task ExecuteAsync(string parameter)
        {
            logger.LogInformation($"Populating ViewModel, parameter is {parameter}");
            switch (parameter)
            {
                case "Get":
                    await GetAllAsync();
                    break;
                case "Start":
                    break;
                case "Stop":
                    break;
                default:
                    break;
            }

        }

        private async Task GetAllAsync()
        {
            await GetPeopleAsync();
            await GetAssignmentsAsync();
            await GetWorkloadsAsync();
        }

        private async Task GetPeopleAsync()
        {
            SelectedPerson = null;
            People = await workloadsService.GetPeopleAsync();
        }

        private async Task GetAssignmentsAsync()
        {
            SelectedAssignment = null;
            Assignments = await workloadsService.GetAssignmentsAsync();
        }

        private async Task GetWorkloadsAsync()
        {
            SelectedWorkload = null;
            Workloads = await workloadsService.GetUnfinishedWorkloadsAsync(0, 0);
        }

        private Task SetWorkloads(int personId = 0, int assignmentId = 0)
        {
            Workloads = workloadsService.GetUnfinishedWorkloadsAsync(personId, assignmentId).Result;
            return Task.CompletedTask;
        }
    }
}
