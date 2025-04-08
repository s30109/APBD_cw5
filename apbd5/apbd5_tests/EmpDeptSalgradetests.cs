using Xunit;
namespace apbd5
{
    public class AdditionalSimpleCommandsTests
    {
        // 1. pracownicy pomiedzy 1000 - 3000
        [Fact]
        public void ShouldReturnEmployeesWithSalaryBetween1000And3000()
        {
            var emps = Database.GetEmps();
            var result = emps.Where(e => e.Sal >= 1000 && e.Sal <= 3000).ToList();
            Assert.Equal(2, result.Count);
        }

        // 2. sorttowanie pracownikow po imieniu w kolejnosci malejacej
        [Fact]
        public void ShouldOrderEmployeesByNameDescending()
        {
            var emps = Database.GetEmps();
            var result = emps.OrderByDescending(e => e.EName).ToList();
            Assert.Equal("WARD", result.First().EName);
        }

        // 3. grupowanie wedlug stanowiska i wybieranie najwiekszej pensji
        [Fact]
        public void ShouldGroupByJobAndSelectMaxSalary()
        {
            var emps = Database.GetEmps();
            var groups = emps.GroupBy(e => e.Job)
                             .Select(g => new { Job = g.Key, MaxSal = g.Max(e => e.Sal) })
                             .ToList();
            var clerkGroup = groups.First(g => g.Job == "CLERK");
            Assert.Equal(5000, clerkGroup.MaxSal);
        }

        // 4. obliczenie sredniego wynagrodzenia w dziale 10
        [Fact]
        public void ShouldCalculateAverageSalaryForDept10()
        {
            var emps = Database.GetEmps();
            var dept10Emps = emps.Where(e => e.DeptNo == 10).ToList();
            var avgSal = dept10Emps.Average(e => e.Sal);
            Assert.Equal(5000, avgSal);
        }

        // 5. laczenie dzialami wykluczajac dzial accounting
        [Fact]
        public void ShouldJoinEmployeesWithDepartmentsExcludingAccounting()
        {
            var emps = Database.GetEmps();
            var depts = Database.GetDepts();
            var result = from emp in emps
                         join dept in depts on emp.DeptNo equals dept.DeptNo
                         where dept.DName != "ACCOUNTING"
                         select new { emp.EName, dept.DName };
            Assert.Equal(3, result.Count());
        }

        // 6. wybierz tylko imiona oraz stanowiska
        [Fact]
        public void ShouldSelectEmployeeNameAndJob()
        {
            var emps = Database.GetEmps();
            var result = emps.Select(e => new { e.EName, e.Job }).ToList();
            Assert.All(result, r =>
            {
                Assert.False(string.IsNullOrEmpty(r.EName));
                Assert.False(string.IsNullOrEmpty(r.Job));
            });
        }

        // 7. przypisanie dzialu dla kazdego pracownika
        [Fact]
        public void ShouldUseSelectManyToRepeatDepartmentNamesForEmployees()
        {
            var emps = Database.GetEmps();
            var depts = Database.GetDepts();
            var result = depts.SelectMany(d => 
                            emps.Where(e => e.DeptNo == d.DeptNo)
                                .Select(e => d.DName))
                            .ToList();
            Assert.Equal(emps.Count(), result.Count);
        }

        // 8. sprawdzenie czy w dziale 40 pracuje ktokolwiek
        [Fact]
        public void ShouldFindAnyEmployeeInDept40()
        {
            var emps = Database.GetEmps();
            bool found = emps.Any(e => e.DeptNo == 40);
            Assert.False(found);
        }

        // 9. wszyscy pracownicy zarabiaja co najmniej 800
        [Fact]
        public void ShouldAllEmployeesEarnAtLeast800()
        {
            var emps = Database.GetEmps();
            bool allAtLeast800 = emps.All(e => e.Sal >= 800);
            Assert.True(allAtLeast800);
        }

        // 10. sprawdzenie czy imie zawiera "KING"
        [Fact]
        public void ShouldContainEmployeeNamedKING()
        {
            var emps = Database.GetEmps();
            var names = emps.Select(e => e.EName).ToList();
            Assert.Contains("KING", names);
        }

        // 11. zmiana wyniku zapytania do tablicy
        [Fact]
        public void ShouldConvertEmployeesToArray()
        {
            var emps = Database.GetEmps();
            var arr = emps.ToArray();
            Assert.Equal(emps.Count, arr.Length);
        }

        // 12. znalezenie pracownika o stanowisku "PRESIDENT"
        [Fact]
        public void ShouldFindPresidentUsingFirstOrDefault()
        {
            var emps = Database.GetEmps();
            var president = emps.FirstOrDefault(e => e.Job == "PRESIDENT");
            Assert.NotNull(president);
            Assert.Equal("KING", president.EName);
        }

        // 13. znalezenie pracownika o imieniu "FORD"
        [Fact]
        public void ShouldFindUniqueEmployeeFORDWithSingleOrDefault()
        {
            var emps = Database.GetEmps();
            var ford = emps.SingleOrDefault(e => e.EName == "FORD");
            Assert.NotNull(ford);
        }

        // 14. sortowanie pracownikow malejaco w zaleznosci od prowizji
        [Fact]
        public void ShouldOrderByCommissionDescendingWithNullsAsZero()
        {
            var emps = Database.GetEmps();
            var ordered = emps.OrderByDescending(e => e.Comm ?? 0).ToList();
            Assert.Equal("WARD", ordered.First().EName);
        }

        // 15. grupowanie wedlug stanowiska i liczenie
        [Fact]
        public void ShouldGroupByJobAndCountEmployees()
        {
            var emps = Database.GetEmps();
            var groups = emps.GroupBy(e => e.Job)
                             .Select(g => new { Job = g.Key, Count = g.Count() })
                             .ToList();
            var groupClerk = groups.FirstOrDefault(g => g.Job == "CLERK");
            Assert.NotNull(groupClerk);
            Assert.Equal(2, groupClerk.Count);
        }

        // 16. obliczanie lacznej pensji
        [Fact]
        public void ShouldCalculateTotalCompensationPerEmployee()
        {
            var emps = Database.GetEmps();
            var results = emps.Select(e => new { e.EName, TotalComp = e.Sal + (e.Comm ?? 0) })
                              .ToList();
            var allen = results.FirstOrDefault(r => r.EName == "ALLEN");
            Assert.NotNull(allen);
            Assert.Equal(1900, allen.TotalComp);
        }

        // 17.odwrocenie posortowanych imion
        [Fact]
        public void ShouldReverseOrderEmployeeNamesAlphabetically()
        {
            var emps = Database.GetEmps();
            var names = emps.Select(e => e.EName).OrderBy(n => n).Reverse().ToList();
            Assert.Equal("WARD", names.First());
        }

        // 18. zlaczenie stanowisk
        [Fact]
        public void ShouldAggregateJobTitlesIntoSingleString()
        {
            var emps = Database.GetEmps();
            var distinctJobs = emps.Select(e => e.Job).Distinct();
            var aggregated = distinctJobs.Aggregate((a, b) => a + "-" + b);
            Assert.Contains("CLERK", aggregated);
            Assert.Contains("SALESMAN", aggregated);
            Assert.Contains("PRESIDENT", aggregated);
        }

        // 19. pomijanie pracownikow dopoki nie trafimy na kogos powyzej 5000
        [Fact]
        public void ShouldSkipWhileEmployeesHaveSalaryNotAbove5000()
        {
            var emps = Database.GetEmps().OrderBy(e => e.Sal).ToList();
            var skipped = emps.SkipWhile(e => e.Sal <= 5000).ToList();
            Assert.Empty(skipped);
        }

        // 20. pobieranie pracownikow dopoki ich wynagrodzenie jest mniejsze niz 5000
        [Fact]
        public void ShouldTakeWhileEmployeesHaveSalaryLessThan5000()
        {
            var emps = Database.GetEmps().OrderBy(e => e.Sal).ToList();
            var taken = emps.TakeWhile(e => e.Sal < 5000).ToList();
            Assert.Equal(3, taken.Count);
        }
    }
}
