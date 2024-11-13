using System.Globalization;

var users = new Dictionary<int, (string, string, string)>() {
    { 1, ("Josip", "Josipovic", "08.11.1998")},
    { 2, ("Marko", "Markovic", "20.12.1996")},
    { 3, ("Frane", "Franic", "15.05.1955")},
    { 4, ("Ivan", "Ivic", "19.07.1963")},
    { 5, ("Petra", "Petric", "03.01.1977")},
};

var usersAccounts = new Dictionary<int, Dictionary<string, decimal>>() {
    {1, new Dictionary<string, decimal>{
            { "ziro", 1254.59m},
            { "tekuci", 256.00m},
            { "prepaid", 100.00m}
        }
    },
    {2, new Dictionary<string, decimal>{
            { "ziro", 557.00m},
            { "tekuci", 984.31m},
            { "prepaid", 20.00m}
        }
    },
    {3, new Dictionary<string, decimal>{
            { "ziro", 5012.95m},
            { "tekuci", -50.00m},
            { "prepaid", 0.00m}
        }
    },
    {4, new Dictionary<string, decimal>{
            { "ziro", 2120.15m},
            { "tekuci", 82.44m},
            { "prepaid", 52.03m}
        }
    },
    {5, new Dictionary<string, decimal>{
            { "ziro", -100.00m},
            { "tekuci", 32.98m},
            { "prepaid", 07.23m}
        }
    },
};

var format = "dd.MM.yyyy";

void wrongEntryy()
{
    Console.Clear();
    Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
    Console.WriteLine();
}

// Kreiranje korisnika
void createUser()
{
    
    Console.Write("Unesite ime novog korisnika: ");
    var name = Console.ReadLine();
    Console.Write("Unesite prezime novog korisnika: ");
    var lastname = Console.ReadLine();
    Console.Write("Unesite datum rodenja novog korisnika: ");
    var birthDate = Console.ReadLine();

    if(!DateTime.TryParseExact(birthDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None,out _))
    {
        wrongEntryy();
        createUser();
        return;
    }

    foreach (var user in users)
    {
        if(user.Value.Item1 == name && user.Value.Item2 == lastname)
        {
            wrongEntryy();
            createUser();
            return;
        }
    }
    users.Add(users.Last().Key + 1, (name?? "", lastname?? "", birthDate?? ""));
    usersAccounts.Add(users.Last().Key + 1, new Dictionary<string, decimal>{
            { "ziro", 0.00m},
            { "tekuci", 100.00m},
            { "prepaid", 0.00m}
        }
    );
    Console.Clear();
    Console.WriteLine("Korisnik uspjesno kreiran:");
    Console.WriteLine($"{name} - {lastname} - {birthDate}");
    Console.WriteLine();
    Console.WriteLine("1 - Kreiraj novog korisnika");
    Console.WriteLine("0 - Pocetni meni");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    while (decision != "1" && decision != "0") {
        wrongEntryy();
        Console.WriteLine("1 - Kreiraj novog korisnika");
        Console.WriteLine("0 - Pocetni meni");
        Console.WriteLine();
        Console.Write("Odaberite radnju: ");
        decision = Console.ReadLine();
    }
    if (decision == "1")
    {
        Console.Clear();
        createUser();
    }
    else if (decision == "0")
    {
        Console.Clear();
        startingMenu();
    }
}

// Brisanje korisnika meni
void deleteUser() {
    
    Console.WriteLine("1 - Po id-u");
    Console.WriteLine("2 - Po imenu i prezimenu");
    Console.WriteLine("0 - Pocetni meni");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            deleteById();
            return;
        case "2":
            Console.Clear();
            deleteByName();
            return;
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            deleteUser();
            return;
    }
}

// Brisanje korisnika prema id-u
void deleteById()
{
    Console.Write("Unesite id korisnika kojeg zelite obrisati: ");
    var id = Console.ReadLine();

    foreach (var user in users)
    {
        if (user.Key == int.Parse(id))
        {
            users.Remove(int.Parse(id));
            Console.Clear();
            Console.WriteLine($"Korisnik id-{id} uspjesno obrisan.");
            Console.WriteLine();
            Console.WriteLine("1 - Obrisite novog korisnika");
            Console.WriteLine("0 - Pocetni meni");
            Console.WriteLine();
            Console.Write("Odaberite radnju: ");
            var decision = Console.ReadLine();
            while (decision != "0" && decision != "1" )
            {
                wrongEntryy();
                Console.WriteLine("1 - Obrisite novog korisnika");
                Console.WriteLine("0 - Pocetni meni");
                Console.WriteLine();
                Console.Write("Odaberite radnju: ");
                decision = Console.ReadLine();
            }

            switch (decision)
            {
                case "1":
                    Console.Clear();
                    deleteById();
                    return;
                case "0":
                    Console.Clear();
                    startingMenu();
                    return;
            }
        }
     }
    wrongEntryy();
    deleteById();
}

// Brisanje korisnika prema imenu i prezimenu
void deleteByName()
{
    Console.Write("Unesite ime korisnika kojeg zelite obrisati: ");
    var name = Console.ReadLine();
    Console.Write("Unesite prezime korisnika kojeg zelite obrisati: ");
    var lastname = Console.ReadLine();

    foreach (var user in users)
    {
        if (user.Value.Item1 == name && user.Value.Item2 == lastname)
        {
            users.Remove(user.Key);
            Console.Clear();
            Console.WriteLine($"Korisnik {name} {lastname} uspjesno obrisan.");
            Console.WriteLine();
            Console.WriteLine("1 - Obrisite novog korisnika");
            Console.WriteLine("0 - Pocetni meni");
            Console.WriteLine();
            Console.Write("Odaberite radnju: ");
            var decision = Console.ReadLine();

            while (decision != "0" && decision != "1")
            {
                wrongEntryy();
                Console.WriteLine("1 - Obrisite novog korisnika");
                Console.WriteLine("0 - Pocetni meni");
                Console.WriteLine();
                Console.Write("Odaberite radnju: ");
                decision = Console.ReadLine();
            }

            switch (decision)
            {
                case "1":
                    Console.Clear();
                    deleteByName();
                    return;
                case "0":
                    Console.Clear();
                    startingMenu();
                    return;
            }
        }
    }
    wrongEntryy();
    deleteByName(); 
}

// Uredivanje korisnika
void updateUser()
{
    Console.Write("Unesite id korisnika cije podatke zelite urediti: ");
    var id = Console.ReadLine();
    Console.Clear();

    foreach (var user in users)
    {
        if (user.Key == int.Parse(id))
        {
            Console.WriteLine("Trenutni podatci korisnika: ");
            Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3}");
            Console.WriteLine();
            Console.Write("Upisite novo ime: ");
            var name = Console.ReadLine();
            Console.Write("Upisite novo prezime: ");
            var lastname = Console.ReadLine();
            Console.Write("Upisite novi datum rodenja: ");
            var birthDate = Console.ReadLine();

            if (!DateTime.TryParseExact(birthDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                wrongEntryy();
                updateUser();
            }
            else 
            {
                users[user.Key] = (name, lastname, birthDate);
            }

            Console.Clear();
            Console.WriteLine($"Korisnik id-{id} uspjesno ureden.");
            Console.WriteLine();
            Console.WriteLine("1 - Uredite novog korisnika");
            Console.WriteLine("0 - Pocetni meni");
            Console.WriteLine();
            Console.Write("Odaberite radnju: ");
            var decision = Console.ReadLine();

            while (decision != "0" && decision != "1")
            {
                wrongEntryy();
                Console.WriteLine("1 - Uredite novog korisnika");
                Console.WriteLine("0 - Pocetni meni");
                Console.WriteLine();
                Console.Write("Odaberite radnju: ");
                decision = Console.ReadLine();
            }

            switch (decision)
            {
                case "1":
                    Console.Clear();
                    updateUser();
                    return;
                case "0":
                    Console.Clear();
                    startingMenu();
                    return;
            }
            
        }
    }

    updateUser();
}

// Pregledavanje korisnika koji imaju barem jedan racun u minusu
void reviewUsersByAccount()
{
    Console.WriteLine("Korisnici koji imaju barem jedan racun u minusu:");
    Console.WriteLine();

    var count = 1;
    while (count <= users.Count)
    {
        if (usersAccounts.ContainsKey(count))
        {
            foreach (var account in usersAccounts[count])
            {
                if (account.Value < 0.00m)
                {
                    Console.WriteLine($"{count} - {users[count].Item1} - {users[count].Item2} - {users[count].Item3}");
                    count++;
                }
            }
        }
        count++;
    }
    Console.WriteLine();
    Console.Write("Unesite 0 za povratak na pocetni meni: ");
    var decision = Console.ReadLine() ;

    switch (decision) 
    {
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            reviewUsersByAccount();
            return;

    }

}

// Pregledavanje korisnika koji su stariji od 30 godina
void reviewUsersByAge() 
{
    Console.WriteLine("Korisnici stariji od 30 godina:");
    Console.WriteLine();

    foreach (var user in users)
    {
        if (DateTime.Parse(user.Value.Item3) < DateTime.Now.AddYears(-30)) 
        {
            Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3}");
        }
    }

    Console.WriteLine();
    Console.Write("Unesite 0 za povratak na pocetni meni: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            reviewUsersByAge();
            return;
    }
}

// Pregledavanje korisnika abecedno po prezimenu
void reviewUsersAlphabetically()
{
    Console.Clear();
    var goBack = false;
    while (!goBack)
    {

        Console.WriteLine("Korisnici poredani abecedno prema prezimenu:");
        Console.WriteLine();

        foreach (var user in users.OrderBy(pair => pair.Value.Item2))
        {
            Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3}");
        }

        Console.WriteLine();
        Console.Write("Unesite 0 za povratak na pocetni meni: ");
        var decision = Console.ReadLine();

        if (decision == "0")
        {
            goBack = true;
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Pogresan unos. Pokusaj ponovno.");
            Console.WriteLine();
        }
    }

    Console.Clear();
    startingMenu();
    return;
}

// Pregledavanje korisnika meni
void reviewUsers()
{
    Console.WriteLine("1 - Ispis korisnika abecedno po prezimenu");
    Console.WriteLine("2 - Ispis korisnika koji imaju vise od 30 godina");
    Console.WriteLine("3 - Korisnici sa racunom u minusu");
    Console.WriteLine("0 - Pocetni meni");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            reviewUsersAlphabetically();
            return;
        case "2": 
            Console.Clear();
            reviewUsersByAge();
            return;
        case "3":
            Console.Clear();
            reviewUsersByAccount();
            return;
        case "0":
            Console.Clear();
            startingMenu();
            return;
        default:
            wrongEntryy();
            reviewUsers();
            return;
    }
}

// Korisnici meni
void usersMenu()
{
    Console.WriteLine("1 - Unos novog korisnika");
    Console.WriteLine("2 - Brisanje korisnika");
    Console.WriteLine("3 - Uredivanje korisnika");
    Console.WriteLine("4 - Pregled korisnika");
    Console.WriteLine("0 - Pocetni meni");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            createUser();
            return;
        case "2":
            Console.Clear();
            deleteUser();
            return;
        case "3":
            Console.Clear();
            updateUser();
            return;
        case "4":
            Console.Clear();
            reviewUsers();
            return;
        case "0":
            Console.Clear();
            startingMenu(); // natrag na pocetni meni
            return;
        default:
            wrongEntryy();
            usersMenu();
            return;
    }
}

// Korisnicki racun meni
void account(int userId) 
{
    Console.WriteLine("1 - Pregled racuna");
    Console.WriteLine("0 - Pocetni meni");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision) 
    {
        case "1": 
            Console.Clear();
            // reviewAccounts funkcija
            return;
        case "0": 
            Console.Clear();
            startingMenu();
            return;
        default: 
            wrongEntryy();
            account(userId);
            return;
    }
}

// Logiranje u racun korisnika
void accountLogin() 
{
    Console.WriteLine("Unesite ime korisnika ili unesite 0 za povratak na pocetni meni: ");
    var name = Console.ReadLine();

    if (name == "0")
    {
        Console.Clear();
        startingMenu();
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Unesite prezime korisnika: ");
    var lastname = Console.ReadLine();
    int userId;

    

    foreach (var user in users)
    {
        if (user.Value.Item1 == name && user.Value.Item2 == lastname) 
        {
            Console.Clear();
            Console.WriteLine($"Dobrodosli {name} {lastname}!");
            Console.WriteLine();
            userId = user.Key;
            account(userId);
            return;
        }
    }

    wrongEntryy();
    accountLogin();
    
}

// Pocetni meni
void startingMenu()
{
    Console.WriteLine("1 - Korisnici");
    Console.WriteLine("2 - Racuni");
    Console.WriteLine("3 - Izlaz iz aplikacije");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            Console.Clear();
            usersMenu();
            return;
        case "2":
            Console.Clear();
            accountLogin();
            return;
        case "3":
            return;
        default:
            wrongEntryy();
            startingMenu();
            return;
    }
}

startingMenu();


