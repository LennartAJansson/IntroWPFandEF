﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

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

        public Visibility ShowStartWorkload
        {
            get => showStartWorkload;
            set
            {
                Set(ref showStartWorkload, value, true);
            }
        }
        private Visibility showStartWorkload;
        public string Comment
        {
            get { return comment; }
            set { Set(ref comment, value); }
        }
        private string comment;

        //The selected values:
        private Person person;
        //When a person is selected then filter Workloads on that person
        public Person SelectedPerson
        {
            get => person;
            set
            {
                Set(ref person, value);
                ShowStartWorkload = SelectedPerson != null && SelectedAssignment != null ? Visibility.Visible : Visibility.Collapsed;
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
                ShowStartWorkload = SelectedPerson != null && SelectedAssignment != null ? Visibility.Visible : Visibility.Collapsed;
                SetWorkloads(SelectedPerson == null ? 0 : SelectedPerson.PersonId, SelectedAssignment == null ? 0 : SelectedAssignment.AssignmentId);
            }
        }

        private Workload workload;
        public Workload SelectedWorkload { get => workload; set => Set(ref workload, value); }

        //The list of values:
        private IEnumerable<Person> people;
        public IEnumerable<Person> People { get => people; set => Set(ref people, value); }

        private IEnumerable<Assignment> assignments;
        public IEnumerable<Assignment> Assignments { get => assignments; set => Set(ref assignments, value); }

        private IEnumerable<Workload> workloads;
        public IEnumerable<Workload> Workloads { get => workloads; set => Set(ref workloads, value); }


        public RelayCommand GetCommand { get; }
        public RelayCommand ClearCommand { get; }
        public RelayCommand CreateWorkloadCommand { get; }
        public RelayCommand<int> StopWorkloadCommand { get; }

        public MainViewModel(ILogger<MainViewModel> logger, IWorkloadService workloadsService)
        {
            this.logger = logger;
            this.workloadsService = workloadsService;
            GetCommand = new RelayCommand(async () => await GetAllAsync());
            ClearCommand = new RelayCommand(async () => await ClearAsync());
            CreateWorkloadCommand = new RelayCommand(async () => await CreateWorkloadAsync());
            StopWorkloadCommand = new RelayCommand<int>(async (int parameter) => await StopWorkloadAsync(parameter));
            ShowStartWorkload = Visibility.Collapsed;
        }

        private Task ClearAsync()
        {
            SelectedAssignment = null;
            SelectedPerson = null;
            return Task.CompletedTask;
        }

        public async Task GetAllAsync()
        {
            await GetPeopleAsync();
            await GetAssignmentsAsync();
            await GetWorkloadsAsync();
        }

        private async Task CreateWorkloadAsync()
        {
            if ((SelectedAssignment != null) && SelectedPerson != null)
            {
                logger.LogInformation($"Starting work for Person {SelectedPerson} and Assignment {SelectedAssignment}");
                await workloadsService.StartWorkloadAsync(SelectedPerson.PersonId, SelectedAssignment.AssignmentId, Comment, DateTimeOffset.UtcNow);
                Comment = "";
                await GetWorkloadsAsync();
            }
        }

        private async Task StopWorkloadAsync(int workloadId)
        {
            if (workloadId != 0)
            {
                logger.LogInformation($"Stopping work for workload id {workloadId}");
                await workloadsService.StopWorkloadAsync(workloadId, DateTimeOffset.UtcNow);
                await GetWorkloadsAsync();
            }
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
            int p = SelectedPerson == null ? 0 : SelectedPerson.PersonId;
            int a = SelectedAssignment == null ? 0 : SelectedAssignment.AssignmentId;
            Workloads = await workloadsService.GetUnfinishedWorkloadsAsync(p, a);
        }

        private Task SetWorkloads(int personId = 0, int assignmentId = 0)
        {
            Workloads = workloadsService.GetUnfinishedWorkloadsAsync(personId, assignmentId).Result;
            return Task.CompletedTask;
        }
    }
}
