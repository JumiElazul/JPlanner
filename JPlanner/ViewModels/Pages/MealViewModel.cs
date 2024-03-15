// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using JPlanner.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace JPlanner.ViewModels.Pages
{
    public partial class MealViewModel : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        [ObservableProperty] private ObservableCollection<Meal> _meals = new ObservableCollection<Meal>();
        [ObservableProperty] private int _calculatedCalories;

        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom() { }

        private void InitializeViewModel()
        {
            Meals.CollectionChanged += OnMealsCollectionChanged;
            InitializeMeals();
            _isInitialized = true;
        }

        private void InitializeMeals()
        {
            Meals.Add(new Meal("Turkey Sandwich", 500, DateTime.Now));
            Meals.Add(new Meal("Redbull", 110, DateTime.Now.AddHours(-1)));
            Meals.Add(new Meal("Fresh Slice best pizza in world", 800, DateTime.Now.AddHours(-5)));
        }

        [RelayCommand]
        private void DeleteSelected(Meal meal)
        {
            if (meal != null)
            {
                Meals.Remove(meal);
            }
        }

        private void OnMealsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalculateTotalCalories();
        }

        private void CalculateTotalCalories()
        {
            CalculatedCalories = Meals.Sum(meal => meal.Calories);
        }
    }
}
