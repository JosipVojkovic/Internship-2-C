using System.Globalization;
using System.Reflection.Metadata;

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

void createUser(bool wrongEntry)
{
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
    }

    Console.Write("Unesite ime novog korisnika: ");
    var name = Console.ReadLine();
    Console.Write("Unesite prezime novog korisnika: ");
    var lastname = Console.ReadLine();
    Console.Write("Unesite datum rodenja novog korisnika: ");
    var birthDate = Console.ReadLine();

    if(!DateTime.TryParseExact(birthDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None,out _))
    {
        createUser(true);
    }

    foreach (var user in users)
    {
        if(user.Value.Item1 == name && user.Value.Item2 == lastname)
        {
            createUser(true);
        }
    }
    users.Add(users.Count + 1, (name?? "", lastname?? "", birthDate?? ""));
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
        Console.Clear();
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
        Console.WriteLine("1 - Kreiraj novog korisnika");
        Console.WriteLine("0 - Pocetni meni");
        Console.WriteLine();
        Console.Write("Odaberite radnju: ");
        decision = Console.ReadLine();
    }
    if (decision == "1")
    {
        createUser(false);
    }
    else if (decision == "0")
    {
        startingMenu(false);
    }
}

void deleteUser(bool wrongEntry) {
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
    }

    Console.WriteLine("1 - Po id-u");
    Console.WriteLine("2 - Po imenu i prezimenu");
    Console.WriteLine("0 - Pocetni meni");
    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            deleteById(false);
            break;
        case "2":
            deleteByName(false);
            break;
        case "0":
            startingMenu(false);
            return;
        default:
            deleteUser(true);
            break;
    }
}

void deleteById(bool wrongEntry)
{
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
    }

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

            switch (decision)
            {
                case "1":
                    deleteById(false);
                    break;
                case "0":
                    startingMenu(false);
                    return;
                default:
                    deleteById(true);
                    break;
            }
        }
     }
    deleteById(true);
}

void deleteByName(bool wrongEntry)
{
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
    }

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

            switch (decision)
            {
                case "1":
                    deleteByName(false);
                    break;
                case "0":
                    startingMenu(false);
                    return;
                default:
                    deleteByName(true);
                    break;
            }
        }
    }
    deleteByName(true); 
}

void updateUser(bool wrongEntry)
{
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
    }

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
                updateUser(true);
            }
            else 
            {
                users[user.Key] = (name, lastname, birthDate);
            }

            Console.Clear();
            Console.WriteLine($"Korisnik id-{id} uspjesno ureden.");
            Console.WriteLine();
            var wrongChoice = true;

            while (wrongChoice)
            {
                Console.WriteLine("1 - Uredite novog korisnika");
                Console.WriteLine("0 - Pocetni meni");
                Console.WriteLine();
                Console.Write("Odaberite radnju: ");
                var decision = Console.ReadLine();

                switch (decision)
                {
                    case "1":
                        wrongChoice = false;
                        updateUser(false);
                        break;
                    case "0":
                        wrongChoice = false;
                        startingMenu(false);
                        return;
                    default:
                        wrongChoice = true;
                        Console.Clear();
                        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
                        Console.WriteLine();
                        break;
                }
            }
        }
    }

    updateUser(true);
}

void reviewUsers()
{
    Console.Clear();
    var goBack = false;
    while (!goBack)
    {

        Console.WriteLine("0 - Pocetni meni");
        Console.WriteLine();

        foreach (var user in users)
        {
            Console.WriteLine($"{user.Key} - {user.Value.Item1} - {user.Value.Item2} - {user.Value.Item3}");
        }

        Console.WriteLine();
        Console.Write("Odaberite radnju: ");
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

    startingMenu(false);

}

void usersMenu(bool wrongEntry)
{
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
    }


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
            createUser(false);
            break;
        case "2":
            deleteUser(false);
            break;
        case "3":
            updateUser(false);
            break;
        case "4":
            reviewUsers();
            break;
        case "0":
            startingMenu(false); // natrag na pocetni meni
            break;
        default:
            usersMenu(true);
            break;
    }
}

void startingMenu(bool wrongEntry)
{
    Console.Clear();

    if (wrongEntry)
    {
        Console.WriteLine("Pogresan unos. Pokusajte ponovno.");
        Console.WriteLine();
    }

    Console.WriteLine("1 - Korisnici");
    Console.WriteLine("2 - Racuni");
    Console.WriteLine("3 - Izlaz iz aplikacije");

    Console.WriteLine();
    Console.Write("Odaberite radnju: ");
    var decision = Console.ReadLine();

    switch (decision)
    {
        case "1":
            usersMenu(false);
            break;
        case "2":
            // funkcija account
            break;
        case "3":
            return;
        default:
            startingMenu(true);
            break;
    }
}

startingMenu(false);


