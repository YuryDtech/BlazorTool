# Changelog

## 2025-07-01
- **Interfejs użytkownika i funkcjonalność:**
    - Na stronie `SchedulerPage` dodano filtr `Device` po `AssetNo` za filtrem `Department`.
    - Umożliwiono edycję `WorkOrderComponent`, dodając Telerik ComboBox dla pola `WOCategory`, `WOLevel`.
- **Zmiany w kodzie:**
    - Dodano nowy punkt końcowy `api/v1/wo/getdict` do `WoController` w celu pobierania kategorii zleceń pracy.
    - Zaktualizowano `ApiServiceClient` do pobierania obiektów `Dict`.
    - Dodano model `Dict.cs`.
- **Poprawki błędów:**
    - Naprawiono problem w `WorkOrderComponent`, gdzie `Department` i `assignedPersons` nie wyświetlały się poprawnie.

## 2025-06-30
- **Interfejs użytkownika i funkcjonalność:**
    - Umożliwiono edycję komponentu `WorkOrderComponent`, dodając Telerik components dla pola `assignedPersons`, `Department` i `Description`.
    - Na stronie `OrderPage` dodano kolumnę 'Device'.
    - Na stronie `SchedulerPage` dodano funkcjonalność `OnClick` dla zleceń (aby przeglądać WorkOrder na liście zleceń) oraz zaimplementowano filtry nagłówków dla zleceń.
- **Zmiany w kodzie:**
    - Utwożono i dodano plik `telerik_manual.md` z instrukcjami użycia komponentów Telerik.

## 2025-06-27
- Dodano stronę Dziennika zmian i zaktualizowano mechanizm logowania.
- Wprowadzono pliki `ChangelogPage.razor` i `CHANGELOG.md` do obsługi informacji o wydaniu.
- Zastąpiono `Debug.Print` przez `Console.WriteLine` w celu ujednolicenia logowania po stronie klienta i serwera.
- Drobne poprawki w interfejsie użytkownika (UI) w plikach `MainLayout.razor` i `OrdersPage.razor`.
- Skonfigurowano kopiowanie pliku `CHANGELOG.md` do katalogu `wwwroot` w celu zapewnienia dostępu po stronie klienta.
- Zaimplementowano wyświetlanie zawartości pliku `CHANGELOG.md` na stronie `ChangelogPage.razor`.
- Skonfigurowano domyślny `HttpClient` w `Program.cs` z `BaseAddress` dla dostępu do plików statycznych.
- Poprawiono atrybut `Link` w pliku `BlazorTool.Client.csproj` dla `CHANGELOG.md`, aby zapewnić jego prawidłowe umieszczenie w `wwwroot`.
- Wycofano zmianę `app.UseBlazorFrameworkFiles()` w `BlazorTool/Program.cs`, ponieważ powodowała problemy z ładowaniem aplikacji.
- Ponownie dodano rejestrację `HttpClient` z `BaseAddress` i sprawdzaniem `serverBaseUrl` w `BlazorTool.Client/Program.cs`.
- **SchedulerPage.razor:**
    - Dodano filtr kategorii urządzeń do siatki nieprzypisanych zleceń.
    - Zaimplementowano pole wyboru (checkbox) do pokazywania/ukrywania zrealizowanych zleceń w siatce nieprzypisanych zleceń.
    - Dodano zdarzenie `OnChange` dla pola wielokrotnego wyboru (multiselect) z przypisanymi osobami, aby wyzwalać natychmiastową aktualizację widoku.
    - Dostosowano układ kontrolek filtrowania i widoku zleceń.

## 2025-06-26
- Ulepszono interfejs użytkownika (UI) Schedulera i zaktualizowano tryb renderowania Blazor.
- Skonfigurowano klienta HttpClient po stronie klienckiej, aby obsługiwał dynamiczny bazowy adres URL serwera.
- Zaktualizowano plik .gitignore.
- Zaimplementowano `PersonController` i zrefaktoryzowano klienta API do obsługi zewnętrznych wywołań.

## 2025-06-25
- W trakcie implementacji: dodawanie filtrów do Schedulera.
- Komponent `WorkOrderComponent`: dodano przewijanie, bardziej kompaktowy widok danych, dynamiczne kolory statusu. Strona `OrdersPage`: wyrównano elementy filtra w jednej linii.

## 2025-06-24
- Dodano filtry na stronie Zleceń, w tym pole wielokrotnego wyboru (multiSelect).
- Naprawiono problemy z autoryzacją (AUTH), dodano różne klienty HTTP.
- Refaktoryzacja pliku `Program.cs`: wstrzykiwanie zależności (DI), dodano `AuthHeaderHandler` do pobierania tokenu, usunięto pobieranie tokenu dla `ApiServiceClient`, dodano `HostAddress` w `appSettings` dla serwera.
- Wyrównano przyciski w edytorze Schedulera.

## 2025-06-23
- Zaimplementowano metody `Save/Delete` w `AppointmentService` oraz `Save/DeleteAppointment` w `ApiServiceClient`.
- Dodano widok terminów w Schedulerze oraz funkcjonalność przesuwania/aktualizowania elementów.
- Zaktualizowano klasę `SchedulerAppointment`, aby dziedziczyła po `WorkOrder`.
- Ustawiono komponent `WorkOrderComponent` jako tylko do odczytu.

## 2025-06-20
- Skonfigurowano układ 3-kolumnowy na stronie Zleceń.
- Włączono otwieranie zleceń po kliknięciu.
- Zaimplementowano sprawdzanie połączenia z API serwera oraz zapis i odczyt ustawień.
- Dodano kontroler `SettingsController`.
- Dodano stronę Ustawień (`Settings.razor`) z opcją `TestApiAddress` i metodą `CheckApiAddress` w `ApiServiceClient`.
- Dodano stronę Zamówienia (`OrderPage`).