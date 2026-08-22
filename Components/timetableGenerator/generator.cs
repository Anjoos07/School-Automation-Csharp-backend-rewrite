using Google.OrTools.Sat;

////////////////////////////////
///      INPUT MODELS       ///
//////////////////////////////

class TimetableInput
{
    public required List<TeacherInput> Teachers { get; set; }
    public int PeriodsPerDay { get; set; }
    public int WorkingDays { get; set; }
    public required List<ClassInput> Classes { get; set; }
}

class TeacherInput
{
    public required string Id { get; set; }
    public required string Name { get; set; }
}

class ClassInput
{
    public int ClassName { get; set; }
    public required string ClassDivision { get; set; }
    public required List<SubjectInput> Subjects { get; set; }
}

class SubjectInput
{
    public required string Name { get; set; }
    public required string Teacher { get; set; }
    public int PeriodsPerWeek { get; set; }
    public int ConsecutivePeriods {get; set;}
}


////////////////////////////////
///      OUTPUT MODELS      ///
//////////////////////////////


class Output
{
    public required List<ClassOutput> Classes { get; set; }
    public required List<TeacherOutput> Teachers { get; set; }
}

class ClassOutput
{
    public int ClassName { get; set; }
    public required string ClassDivision { get; set; }
    public required List<List<object?>> Timetable { get; set; }
}

class TeacherOutput
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required List<List<object?>> Timetable { get; set; }
}



  ///////////////////////////////
 //  INPUT CLASS DEFINITION   //
///////////////////////////////


class Teacher
{
    public Teacher(string Id, string Name, List<List<object?>> Timetable)
    {
        this.Id = Id;
        this.Name = Name;
        this.Timetable = Timetable;
    }
    public string Id {get; set;}
    public string Name {get; set;}
    public List<List<object?>> Timetable {get; set;}
}

class Subject
{
    public Subject (string Name, Teacher Teacher, int PeriodsPerWeek, int ConsecutivePeriods)
    {
        this.Name = Name;
        this.Teacher = Teacher;
        this.PeriodsPerWeek = PeriodsPerWeek;
        this.ConsecutivePeriods = ConsecutivePeriods;
    }
    public string Name {get; set;}
    public Teacher Teacher {get; set;}
    public int PeriodsPerWeek {get; set;}
    public int ConsecutivePeriods {get; set;}
}

class SchoolClass
{
    public SchoolClass(int Name, string Division, List<Subject> Subjects, List<List<object?>> Timetable)
    {
        this.Name = Name;
        this.Division = Division;
        this.Subjects = Subjects;
        this.Timetable = Timetable;
    }
    public int Name {get; set;}
    public string Division {get; set;}
    public List<Subject> Subjects {get; set;}
    public List<List<object?>> Timetable {get; set;}
}

class School
{
    public School(int PeriodsPerDay, int WorkingDays, List<SchoolClass> Classes, List<Teacher> Teachers)
    {
        this.PeriodsPerDay = PeriodsPerDay;
        this.WorkingDays = WorkingDays;
        this.Classes = Classes;
        this.Teachers = Teachers;
    }
    public int PeriodsPerDay {get; set;}
    public int WorkingDays {get; set;}
    public List<SchoolClass> Classes {get; set;}
    public List<Teacher> Teachers {get; set;}
}

  ///////////////////////////////
 // process class definition //
/////////////////////////////

class Lesson
{
    public Lesson(SchoolClass SchoolClass, Subject Subject)
    {
        this.SchoolClass = SchoolClass;
        this.Subject = Subject;
    }
    public SchoolClass SchoolClass {get; set;}
    public Subject Subject {get; set;}
}

  /////////////////////////
 //  HELPER FUNCTIONS  //
///////////////////////

static class GeneralHelper
{
    public static List<List<object?>> CreateNullTable(int days, int periods)
    {
        List<List<object?>> table = new();

        for (int day = 0; day < days; day++)
        {
            List<object?> row = new();

            for (int period = 0; period < periods; period++)
            {
                row.Add(null);
            }

            table.Add(row);
        }

        return table;
    }

    public static List<Lesson> CreateLesson(School school)
    {
        List<Lesson> lessons = new();

        foreach(SchoolClass schoolClass in school.Classes)
        {
            foreach(Subject subject in schoolClass.Subjects)
            {
                if (subject.PeriodsPerWeek % subject.ConsecutivePeriods != 0)
                {
                    throw new ArgumentException(
                        $"{subject.Name}: periodsPerWeek must be divisible by consecutivePeriods"
                    );
                }
                int blocks = subject.PeriodsPerWeek / subject.ConsecutivePeriods;
                for (int _ = 0; _ < blocks; _++)
                {
                    lessons.Add(new Lesson(schoolClass, subject));
                }
            }
        }
        return lessons;
    }
}

static class SolverHelper
{
    public static List<int> ValidStartPeriods(int duration, School school)
    {
        int maxStart = school.PeriodsPerDay - duration;

        if (duration == 2)
        {
            return Enumerable.Range(0, maxStart / 2 + 1).Select(x => x * 2).ToList();
        }
        return Enumerable.Range(0, maxStart + 1).ToList();
    } 
}

static class Processing
{
    public static School TimetableProcessInput(TimetableInput data)
    {
        List<Teacher> teachers = new();
        int periodsPerDay = data.PeriodsPerDay;
        int workingDays = data.WorkingDays;
        int requiredPeriods = workingDays * periodsPerDay;
        foreach(TeacherInput teacher in data.Teachers)
        {
            string tname = teacher.Name;
            string tid = teacher.Id;
            teachers.Add(new Teacher(tid, tname, GeneralHelper.CreateNullTable(workingDays, periodsPerDay)));
        }

        //teacherLookup defined below is a hash to look up teacher objects from id
        //which is needed while assigning teachers to subjects in the Subject object
        var teacherLookup = teachers.ToDictionary(teacher => teacher.Id);

        List<SchoolClass> classes = new();

        foreach (ClassInput indieClass in data.Classes)
        {
            int className = indieClass.ClassName;
            string classDivision = indieClass.ClassDivision;
            List<Subject> subjects = new();

            foreach(SubjectInput subject in indieClass.Subjects)
            {
                string subname = subject.Name;
                Teacher subteacher = teacherLookup[subject.Teacher];
                int periodsPerWeek = subject.PeriodsPerWeek;
                int consecutivePeriods = subject.ConsecutivePeriods;
                subjects.Add(new Subject(subname, subteacher, periodsPerWeek, consecutivePeriods));
            }
            int total = subjects.Sum(subject => subject.PeriodsPerWeek);

            if (total != requiredPeriods)
            {
                throw new ArgumentException(
                    $"{className}{classDivision} has {total} periods " +
                    $"but needs {requiredPeriods}"
                );           
            }
            classes.Add(new SchoolClass(className, classDivision, subjects, GeneralHelper.CreateNullTable(workingDays, periodsPerDay)));
        }
        return new School(periodsPerDay, workingDays, classes, teachers);

    }
}

static class SolverMain
{
    public static void CpSatSolver(List<Lesson> lessons, School school)
    {
        CpModel model = new();
        Dictionary<(int lessonIndex, int day, int startPeriod), BoolVar> lessonStarts = new();

        for (int lessonIndex = 0; lessonIndex < lessons.Count; lessonIndex++)
        {
            Lesson lesson = lessons[lessonIndex];

            int duration = lesson.Subject.ConsecutivePeriods;

            for (int day = 0; day < school.WorkingDays; day++)
            {
                foreach (int startPeriod in SolverHelper.ValidStartPeriods(duration, school))
                {
                    lessonStarts[(lessonIndex, day, startPeriod)] =
                        model.NewBoolVar(
                            $"L{lessonIndex}_D{day}_P{startPeriod}"
                        );
                }
            }
        }

        //CONSTRAINT 1 EACH LESSON MUST BE PLACED ONLY ONCE


        for (int lessonIndex = 0; lessonIndex < lessons.Count(); lessonIndex++)
        {
            Lesson lesson = lessons[lessonIndex];
            int duration = lesson.Subject.ConsecutivePeriods;
            
        var vars = new List<BoolVar>();

        for (int day = 0; day < school.WorkingDays; day++)
        {
            foreach (int startPeriod in SolverHelper.ValidStartPeriods(duration, school))
            {
                vars.Add(lessonStarts[(lessonIndex, day, startPeriod)]);
            }
        }

        model.AddExactlyOne(vars);
        }


        // CONSTRAINT 2: ONE TEACHER MUST NOT GET MULTIPLE LESSONS ON THE SAME PERIOD

        foreach (Teacher teacher in school.Teachers)
        {
            List<int> teacherLessonsIndices = new();

            for (int lessonIndex = 0; lessonIndex < lessons.Count; lessonIndex++)
            {
                if (lessons[lessonIndex].Subject.Teacher == teacher)
                {
                    teacherLessonsIndices.Add(lessonIndex);
                }
            }

            for (int day = 0; day < school.WorkingDays; day++)
            {
                for (int occupiedPeriod = 0;
                    occupiedPeriod < school.PeriodsPerDay;
                    occupiedPeriod++)
                {
                    List<BoolVar> occupiedLessons = new();

                    foreach (int lessonIndex in teacherLessonsIndices)
                    {
                        int duration = lessons[lessonIndex].Subject.ConsecutivePeriods;

                        foreach (int startPeriod in SolverHelper.ValidStartPeriods(duration, school))
                        {
                            if (startPeriod <= occupiedPeriod &&
                                occupiedPeriod < startPeriod + duration)
                            {
                                occupiedLessons.Add(
                                    lessonStarts[(lessonIndex, day, startPeriod)]
                                );
                            }
                        }
                    }

                    model.Add(LinearExpr.Sum(occupiedLessons) <= 1);
                }
            }
        }

        // CONSTRAINT 3: ONLY 1 ASSIGNMENT PER PERIOD FOR A GIVEN CLASS

        foreach (SchoolClass schoolClass in school.Classes)
        {
            List<int> classLessonIndices = new();

            for (int lessonIndex = 0; lessonIndex < lessons.Count; lessonIndex++)
            {
                if (lessons[lessonIndex].SchoolClass == schoolClass)
                {
                    classLessonIndices.Add(lessonIndex);
                }
            }

            for (int day = 0; day < school.WorkingDays; day++)
            {
                for (int occupiedPeriod = 0;
                    occupiedPeriod < school.PeriodsPerDay;
                    occupiedPeriod++)
                {
                    List<BoolVar> occupiedLessons = new();

                    foreach (int lessonIndex in classLessonIndices)
                    {
                        int duration = lessons[lessonIndex]
                            .Subject
                            .ConsecutivePeriods;

                        foreach (int startPeriod in SolverHelper.ValidStartPeriods(duration, school))
                        {
                            if (startPeriod <= occupiedPeriod &&
                                occupiedPeriod < startPeriod + duration)
                            {
                                occupiedLessons.Add(
                                    lessonStarts[(lessonIndex, day, startPeriod)]
                                );
                            }
                        }
                    }

                    model.Add(LinearExpr.Sum(occupiedLessons) == 1);
                }
            }
        }
        // ##############################################
        // #      THIS SECTION COVERS SOFT CONSTRAINTS  #
        // ##############################################


        // ============================================================
        // CONSTRAINT 1:
        // MINIMIZING CONSECUTIVE PERIODS FOR EACH CLASS
        // IN A GIVEN DAY
        // ============================================================

        List<IntVar> classPenalties = new();

        foreach (SchoolClass schoolClass in school.Classes)
        {
            foreach (Subject subject in schoolClass.Subjects)
            {
                for (int day = 0; day < school.WorkingDays; day++)
                {
                    List<BoolVar> subjectLessons = new();

                    for (int lessonIndex = 0;
                        lessonIndex < lessons.Count;
                        lessonIndex++)
                    {
                        Lesson lesson = lessons[lessonIndex];

                        if (lesson.SchoolClass == schoolClass &&
                            lesson.Subject == subject)
                        {
                            int duration = lesson.Subject.ConsecutivePeriods;

                            for (int occupiedPeriod = 0;
                                occupiedPeriod < school.PeriodsPerDay;
                                occupiedPeriod++)
                            {
                                foreach (int startPeriod in
                                    SolverHelper.ValidStartPeriods(duration, school))
                                {
                                    if (startPeriod <= occupiedPeriod &&
                                        occupiedPeriod < startPeriod + duration)
                                    {
                                        subjectLessons.Add(
                                            lessonStarts[
                                                (lessonIndex, day, startPeriod)
                                            ]
                                        );
                                    }
                                }
                            }
                        }
                    }

                    LinearExpr count = LinearExpr.Sum(subjectLessons);

                    IntVar penalty = model.NewIntVar(
                        0,
                        school.PeriodsPerDay,
                        $"{schoolClass.Name}_{subject.Name}_{day}"
                    );

                    model.AddMaxEquality(
                        penalty,
                        new LinearExpr[]
                        {
                            LinearExpr.Constant(0),
                            count - 2
                        }
                    );

                    classPenalties.Add(penalty);
                }
            }
        }


        // ============================================================
        // CONSTRAINT 2:
        // MINIMIZING TEACHER TEACHING DENSITY
        // ============================================================

        List<IntVar> teacherPenalties = new();

        foreach (Teacher teacher in school.Teachers)
        {
            List<int> teacherLessonsIndices = new();

            for (int lessonIndex = 0;
                lessonIndex < lessons.Count;
                lessonIndex++)
            {
                if (lessons[lessonIndex].Subject.Teacher == teacher)
                {
                    teacherLessonsIndices.Add(lessonIndex);
                }
            }

            for (int day = 0;
                day < school.WorkingDays;
                day++)
            {
                // Look at every window of 4 consecutive periods
                for (int windowStart = 0;
                    windowStart < school.PeriodsPerDay - 3;
                    windowStart++)
                {
                    List<LinearExpr> teachingPeriods = new();

                    for (int occupiedPeriod = windowStart;
                        occupiedPeriod < windowStart + 4;
                        occupiedPeriod++)
                    {
                        List<BoolVar> occupiedLessons = new();

                        foreach (int lessonIndex in teacherLessonsIndices)
                        {
                            int duration =
                                lessons[lessonIndex]
                                    .Subject
                                    .ConsecutivePeriods;

                            foreach (int startPeriod in
                                SolverHelper.ValidStartPeriods(duration, school))
                            {
                                if (startPeriod <= occupiedPeriod &&
                                    occupiedPeriod < startPeriod + duration)
                                {
                                    occupiedLessons.Add(
                                        lessonStarts[
                                            (lessonIndex, day, startPeriod)
                                        ]
                                    );
                                }
                            }
                        }

                        teachingPeriods.Add(
                            LinearExpr.Sum(occupiedLessons)
                        );
                    }

                    // Total periods taught in this 4-period window
                    LinearExpr totalTeaching =
                        LinearExpr.Sum(teachingPeriods);

                    IntVar penalty = model.NewIntVar(
                        0,
                        4,
                        ""
                    );

                    model.AddMaxEquality(
                        penalty,
                        new LinearExpr[]
                        {
                            LinearExpr.Constant(0),
                            totalTeaching - 3
                        }
                    );

                    teacherPenalties.Add(penalty);
                }
            }
        }


        // ============================================================
        // MINIMIZE ALL PENALTIES
        // ============================================================

        List<IntVar> allPenalties = new();

        allPenalties.AddRange(classPenalties);
        allPenalties.AddRange(teacherPenalties);

        model.Minimize(
            LinearExpr.Sum(allPenalties)
        );


        // ============================================================
        // RUNNING THE MODEL
        // ============================================================

        CpSolver solver = new();

        CpSolverStatus status = solver.Solve(model);

        if (status != CpSolverStatus.Optimal &&
            status != CpSolverStatus.Feasible)
        {
            Console.WriteLine(status);
            return;
        }


        // ============================================================
        // GENERATE THE TIMETABLE
        // ============================================================

        for (int lessonIndex = 0;
            lessonIndex < lessons.Count;
            lessonIndex++)
        {
            Lesson lesson = lessons[lessonIndex];

            int duration = lesson.Subject.ConsecutivePeriods;

            for (int day = 0;
                day < school.WorkingDays;
                day++)
            {
                foreach (int startPeriod in
                    SolverHelper.ValidStartPeriods(duration, school))
                {
                    if (solver.BooleanValue(
                        lessonStarts[
                            (lessonIndex, day, startPeriod)
                        ]))
                    {
                        for (int offset = 0;
                            offset < duration;
                            offset++)
                        {
                            int period = startPeriod + offset;

                            // Class timetable
                            lesson.SchoolClass.Timetable[day][period] =
                                new
                                {
                                    Subject = lesson.Subject.Name,
                                    Teacher = lesson.Subject.Teacher.Name
                                };

                            // Teacher timetable
                            lesson.Subject.Teacher.Timetable[day][period] =
                                new
                                {
                                    Subject = lesson.Subject.Name,
                                    ClassName = lesson.SchoolClass.Name,
                                    ClassDivision = lesson.SchoolClass.Division
                                };
                        }
                    }
                }
            }
        }
    }
}

static class OutputProcessing
{
    static class OutputHelper
    {
        public static Output GenerateOutput(School data)
        {
            Output output = new()
            {
                Classes = new(),
                Teachers = new()
            };

            foreach (SchoolClass schoolClass in data.Classes)
            {
                output.Classes.Add(new ClassOutput
                {
                    ClassName = schoolClass.Name,
                    ClassDivision = schoolClass.Division,
                    Timetable = schoolClass.Timetable
                });
            }

            foreach (Teacher teacher in data.Teachers)
            {
                output.Teachers.Add(new TeacherOutput
                {
                    Id = teacher.Id,
                    Name = teacher.Name,
                    Timetable = teacher.Timetable
                });
            }

            return output;
        }
    }

    public static Output TimeTableGen(TimetableInput inputData)
    {
        School data = Processing.TimetableProcessInput(inputData);

        List<Lesson> lessons = GeneralHelper.CreateLesson(data);

        // Your CP-SAT function
        SolverMain.CpSatSolver(lessons, data);

        Output output = OutputHelper.GenerateOutput(data);

        return output;
    }
}