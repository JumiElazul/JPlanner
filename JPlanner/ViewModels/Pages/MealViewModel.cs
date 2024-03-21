// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using JPlanner.Database;
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
            InitializeMealCollection();
            _isInitialized = true;
        }

        private void InitializeMealCollection()
        {
            List<MealEntry> mealEntries = SQLiteHandler.GetMealEntriesForUser("DefaultUser");
            Meals = new ObservableCollection<MealEntry>(mealEntries);

            Meals.CollectionChanged += OnMealsCollectionChanged;
            CalculateTotalCalories();
        }

        [RelayCommand]
        private void DeleteSelected(MealEntry meal)
        {
            if (meal != null)
            {
                Meals.Remove(meal);
                SQLiteHandler.DeleteMealForUser("DefaultUser", meal);
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
        private void SubmitMeal(string user)
        {
            if (!String.IsNullOrWhiteSpace(MealInfo) && !String.IsNullOrWhiteSpace(CalorieInfo))
            {
                bool converted = int.TryParse(CalorieInfo, out int calories);

                if (converted)
                {
                    MealEntry newMeal = new MealEntry(MealInfo, calories, DateTime.Now);
                    Meals.Add(newMeal);
                    SQLiteHandler.CreateMealForUser("DefaultUser", newMeal);

                    ClearInputFields();
                }
            }
        }

        private void ClearInputFields()
        {
            MealInfo = String.Empty;
            CalorieInfo = String.Empty;
        }
    }
}
