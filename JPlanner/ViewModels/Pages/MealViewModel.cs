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
        [ObservableProperty] private ObservableCollection<MealEntry> _meals = new ObservableCollection<MealEntry>();
        [ObservableProperty] private int _calculatedCalories;
        [ObservableProperty] private string _mealInfo;
        [ObservableProperty] private string _calorieInfo;

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
            Meals.Add(new MealEntry("Turkey Sandwich", 500, DateTime.Now));
            Meals.Add(new MealEntry("Redbull", 110, DateTime.Now.AddHours(-1)));
            Meals.Add(new MealEntry("Fresh Slice best pizza in world", 800, DateTime.Now.AddHours(-5)));
        }

        [RelayCommand]
        private void DeleteSelected(MealEntry meal)
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

        [RelayCommand]
        private void SubmitMeal()
        {
            if (!String.IsNullOrWhiteSpace(MealInfo) && !String.IsNullOrWhiteSpace(CalorieInfo))
            {
                bool converted = int.TryParse(CalorieInfo, out int calories);

                if (converted)
                {
                    Meals.Add(new MealEntry(MealInfo, calories, DateTime.Now));
                    MealInfo = String.Empty;
                    CalorieInfo = String.Empty;
                }
            }
        }
    }
}
