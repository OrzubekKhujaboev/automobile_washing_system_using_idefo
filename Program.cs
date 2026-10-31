using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace AvtoYuvishBoshqaruvApp;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainWindow());
    }
}

public class CarOrder
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Customer { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Plate { get; set; } = "";
    public string Car { get; set; } = "";
    public string Service { get; set; } = "";
    public decimal Price { get; set; }
    public int Stage { get; set; } = 0;
    public bool QualityApproved { get; set; }
    public bool Paid { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class MainWindow : Form
{
    private static readonly Color Navy = Color.FromArgb(19, 32, 59);
    private static readonly Color Blue = Color.FromArgb(41, 99, 238);
    private static readonly Color Background = Color.FromArgb(245, 248, 252);
    private static readonly Color Muted = Color.FromArgb(95, 113, 141);
    private static readonly Dictionary<string, decimal> Prices = new()
    {
        ["Yengil yuvish"] = 30000,
        ["To'liq yuvish"] = 50000,
        ["Salon tozalash"] = 80000,
        ["Premium kompleks"] = 120000
    };
    private static readonly string[] Stages =
    ["A1 · Qabul qilindi", "A2 · Tayyorlandi", "A3 · Yuvildi", "A4 · Sifat tekshiruvi", "Yakunlandi"];
    private readonly List<CarOrder> orders = [];
    private readonly string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AvtoYuvishBoshqaruvApp");
    private readonly DataGridView grid = new();
    private readonly TextBox customer = new(), phone = new(), plate = new(), car = new(), search = new();
    private readonly ComboBox service = new();
    private readonly Label priceLabel = new(), statistics = new(), selectionInfo = new();
    private readonly Button advance = new(), receipt = new(), approve = new(), paid = new();
    private readonly CheckBox qualityCheck = new(), paidCheck = new();
    private readonly TabControl tabs = new();
    private CarOrder? selected;

    public MainWindow()
    {
        Text = "AVTO YUVISH | Boshqaruv tizimi";
        MinimumSize = new Size(1050, 700);
        Size = new Size(1260, 820);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Background;
        Font = new Font("Segoe UI", 10);
        LoadOrders();
        BuildUi();
        RefreshGrid();
    }

    private void BuildUi()
    {
        var header = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.White, Padding = new Padding(24, 15, 24, 8) };
        var heading = new Label { Text = "AVTO YUVISH  |  Boshqaruv tizimi", ForeColor = Navy, Font = new Font("Segoe UI", 20, FontStyle.Bold), Dock = DockStyle.Top, Height = 44 };
        var sub = new Label { Text = "IDEF0 asosidagi ish jarayoni: A1 Buyurtma  →  A2 Tayyorlash  →  A3 Yuvish  →  A4 Tekshirish va topshirish", ForeColor = Muted, Dock = DockStyle.Top, Height = 30 };
        header.Controls.Add(sub); header.Controls.Add(heading);
        Controls.Add(header);
        tabs.Dock = DockStyle.Fill;
        tabs.Padding = new Point(20, 10);
        tabs.TabPages.Add(MakeOrdersTab());
        tabs.TabPages.Add(MakeNewTab());
        tabs.TabPages.Add(MakeInfoTab());
        Controls.Add(tabs);
        tabs.BringToFront();
    }

    private TabPage MakeOrdersTab()
    {
        var tab = new TabPage("Buyurtmalar va ish jarayoni") { BackColor = Background, Padding = new Padding(20) };
        var top = new TableLayoutPanel { Dock = DockStyle.Top, Height = 112, ColumnCount = 2, RowCount = 2 };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
        statistics.Font = new Font("Segoe UI", 12, FontStyle.Bold); statistics.ForeColor = Navy;
        statistics.Dock = DockStyle.Fill; statistics.TextAlign = ContentAlignment.MiddleLeft;
        top.Controls.Add(statistics, 0, 0); top.SetColumnSpan(statistics, 2);
        search.PlaceholderText = "Mijoz, raqam yoki xizmat bo'yicha qidirish...";
        search.Dock = DockStyle.Fill; search.TextChanged += (_, _) => RefreshGrid();
        top.Controls.Add(search, 0, 1);
        var newButton = MakeButton("+ Yangi buyurtma", Blue);
        newButton.Dock = DockStyle.Fill;
        newButton.Click += (_, _) => tabs.SelectedIndex = 1;
        top.Controls.Add(newButton, 1, 1);
        tab.Controls.Add(top);

        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 155, Padding = new Padding(0, 12, 0, 0) };
        selectionInfo.Dock = DockStyle.Top; selectionInfo.Height = 35; selectionInfo.ForeColor = Navy;
        selectionInfo.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        bottom.Controls.Add(selectionInfo);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, WrapContents = true };
        advance.Text = "Keyingi bosqichga o'tkazish";
        approve.Text = "Sifat tasdiqlash";
        paid.Text = "To'lovni qabul qilish";
        receipt.Text = "Chekni saqlash";
        foreach (var b in new[] { advance, approve, paid, receipt })
        {
            b.BackColor = Blue; b.ForeColor = Color.White; b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0; b.AutoSize = true; b.MinimumSize = new Size(175, 42);
            b.Margin = new Padding(0, 8, 12, 0); b.Enabled = false;
            actions.Controls.Add(b);
        }
        advance.Click += (_, _) => AdvanceOrder();
        approve.Click += (_, _) => ApproveQuality();
        paid.Click += (_, _) => AcceptPayment();
        receipt.Click += (_, _) => SaveReceipt();
        bottom.Controls.Add(actions);
        var note = new Label { Text = "Ish tartibi: qabul → tayyorlash → yuvish → sifat nazorati → to'lov → topshirish", ForeColor = Muted, Dock = DockStyle.Bottom, Height = 28 };
        bottom.Controls.Add(note);
        tab.Controls.Add(bottom);

        grid.Dock = DockStyle.Fill;
        grid.AllowUserToAddRows = false; grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true; grid.MultiSelect = false; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.RowHeadersVisible = false; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = Color.White; grid.BorderStyle = BorderStyle.None;
        grid.RowTemplate.Height = 34; grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(231, 239, 254);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Navy;
        grid.Columns.Add("id", "№"); grid.Columns.Add("date", "Sana"); grid.Columns.Add("customer", "Mijoz");
        grid.Columns.Add("plate", "Davlat raqami"); grid.Columns.Add("car", "Avtomobil");
        grid.Columns.Add("service", "Xizmat"); grid.Columns.Add("price", "Narx (so'm)");
        grid.Columns.Add("stage", "Holat"); grid.Columns.Add("payment", "To'lov");
        grid.SelectionChanged += (_, _) => SelectGridOrder();
        tab.Controls.Add(grid); grid.BringToFront();
        return tab;
    }

    private TabPage MakeNewTab()
    {
        var tab = new TabPage("A1 · Yangi buyurtma") { BackColor = Background, Padding = new Padding(40, 26, 40, 25) };
        var form = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, RowCount = 8, BackColor = Color.White, Padding = new Padding(25) };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 210));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddField(form, 0, "Mijozning F.I.Sh.", customer);
        AddField(form, 1, "Telefon raqami", phone);
        AddField(form, 2, "Davlat raqami", plate);
        AddField(form, 3, "Avtomobil modeli", car);
        service.DropDownStyle = ComboBoxStyle.DropDownList;
        service.Items.AddRange(Prices.Keys.Cast<object>().ToArray());
        service.SelectedIndexChanged += (_, _) => priceLabel.Text = service.SelectedItem is string s ? $"Narx: {Prices[s]:N0} so'm" : "";
        service.SelectedIndex = 0;
        AddField(form, 4, "Xizmat turi", service);
        priceLabel.Text = $"Narx: {Prices[(string)service.SelectedItem!]:N0} so'm";
        priceLabel.ForeColor = Blue; priceLabel.Font = new Font("Segoe UI", 13, FontStyle.Bold);
        AddField(form, 5, "Xizmat narxi", priceLabel);
        var save = MakeButton("Buyurtmani saqlash (A1)", Blue);
        save.Height = 48; save.Dock = DockStyle.Fill;
        save.Click += (_, _) => CreateOrder();
        form.Controls.Add(save, 1, 6);
        tab.Controls.Add(form);
        return tab;
    }

    private TabPage MakeInfoTab()
    {
        var tab = new TabPage("Jarayon haqida") { BackColor = Background, Padding = new Padding(36) };
        var info = new Label
        {
            Dock = DockStyle.Fill, AutoSize = false, ForeColor = Navy, Font = new Font("Segoe UI", 13),
            Text = "AVTOMOBIL YUVISH XIZMATI — IDEF0 ASOSIDA\n\n" +
                   "A1 — Buyurtmani qabul qilish: mijoz, avtomobil va xizmat turi ro'yxatga olinadi.\n\n" +
                   "A2 — Tayyorlash: avtomobil yuvishga tayyor holatga keltiriladi.\n\n" +
                   "A3 — Yuvish: tanlangan xizmat bajariladi.\n\n" +
                   "A4 — Sifat nazorati va topshirish: sifat tasdiqlanadi, to'lov olinadi va chek beriladi.\n\n" +
                   "Saqlash: buyurtmalar kompyuterda avtomatik JSON faylga yoziladi.\n" +
                   "Dastur demo/o'quv loyihasi bo'lib, haqiqiy fiskal to'lov tizimiga ulanmagan."
        };
        tab.Controls.Add(info);
        return tab;
    }

    private static Button MakeButton(string caption, Color color) => new()
    {
        Text = caption, BackColor = color, ForeColor = Color.White,
        FlatStyle = FlatStyle.Flat, Height = 42, Font = new Font("Segoe UI", 10, FontStyle.Bold)
    };

    private static void AddField(TableLayoutPanel table, int row, string title, Control control)
    {
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 57));
        var label = new Label { Text = title, ForeColor = Navy, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(5, 9, 20, 9);
        table.Controls.Add(label, 0, row);
        table.Controls.Add(control, 1, row);
    }

    private void CreateOrder()
    {
        if (string.IsNullOrWhiteSpace(customer.Text) || string.IsNullOrWhiteSpace(plate.Text) || string.IsNullOrWhiteSpace(car.Text))
        {
            MessageBox.Show("Mijoz ismi, avtomobil modeli va davlat raqamini kiriting.", "Ma'lumot yetarli emas");
            return;
        }
        var option = (string)service.SelectedItem!;
        var item = new CarOrder
        {
            Id = orders.Count == 0 ? 1 : orders.Max(x => x.Id) + 1,
            Customer = customer.Text.Trim(), Phone = phone.Text.Trim(), Plate = plate.Text.Trim().ToUpperInvariant(),
            Car = car.Text.Trim(), Service = option, Price = Prices[option]
        };
        orders.Add(item); SaveOrders();
        customer.Clear(); phone.Clear(); plate.Clear(); car.Clear();
        search.Clear(); selected = item; tabs.SelectedIndex = 0;
        RefreshGrid(item.Id);
        MessageBox.Show($"Buyurtma №{item.Id} qabul qilindi!", "A1 — Muvaffaqiyatli");
    }

    private void RefreshGrid(int? highlightId = null)
    {
        int? currentId = highlightId ?? selected?.Id;
        var term = search.Text.Trim();
        grid.Rows.Clear();
        foreach (var order in orders.OrderByDescending(x => x.Id).Where(x =>
            string.IsNullOrEmpty(term) ||
            x.Customer.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            x.Plate.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            x.Service.Contains(term, StringComparison.OrdinalIgnoreCase)))
        {
            int i = grid.Rows.Add(order.Id, order.CreatedAt.ToString("dd.MM.yyyy HH:mm"), order.Customer,
                order.Plate, order.Car, order.Service, order.Price.ToString("N0"), Stages[order.Stage], order.Paid ? "To'langan" : "Kutilmoqda");
            grid.Rows[i].Tag = order;
            if (order.Stage == 4) grid.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(228, 248, 234);
        }
        grid.ClearSelection();
        foreach (DataGridViewRow row in grid.Rows)
            if (row.Tag is CarOrder o && o.Id == currentId) { row.Selected = true; break; }
        statistics.Text = $"Jami buyurtmalar: {orders.Count}     |     Faol: {orders.Count(x => x.Stage < 4)}     |     Yakunlangan: {orders.Count(x => x.Stage == 4)}     |     Tushum: {orders.Where(x => x.Paid).Sum(x => x.Price):N0} so'm";
        SelectGridOrder();
    }

    private void SelectGridOrder()
    {
        selected = grid.SelectedRows.Count == 1 ? grid.SelectedRows[0].Tag as CarOrder : null;
        selectionInfo.Text = selected is null ? "Davom ettirish uchun buyurtmani tanlang" :
            $"Buyurtma №{selected.Id} | {selected.Customer} | {Stages[selected.Stage]}";
        advance.Enabled = selected is { Stage: < 4 };
        advance.Text = selected?.Stage switch
        {
            0 => "A2 · Tayyorlandi", 1 => "A3 · Yuvildi", 2 => "A4 · Tekshiruvga", 3 => "Yakunlash / topshirish", _ => "Yakunlangan"
        };
        approve.Enabled = selected is { Stage: 3, QualityApproved: false };
        paid.Enabled = selected is { Stage: 3, Paid: false };
        receipt.Enabled = selected is { Stage: 4 };
    }

    private void AdvanceOrder()
    {
        if (selected is null || selected.Stage >= 4) return;
        if (selected.Stage == 3 && (!selected.QualityApproved || !selected.Paid))
        {
            MessageBox.Show("Yakunlashdan oldin sifatni tasdiqlang va to'lovni qabul qiling.", "A4 — Tekshiruv");
            return;
        }
        if (selected.Stage == 3)
        {
            selected.CompletedAt = DateTime.Now;
        }
        selected.Stage++;
        int id = selected.Id;
        SaveOrders(); RefreshGrid(id);
    }

    private void ApproveQuality()
    {
        if (selected is not { Stage: 3 }) return;
        selected.QualityApproved = true;
        int id = selected.Id; SaveOrders(); RefreshGrid(id);
        MessageBox.Show("Sifat tekshiruvi tasdiqlandi.", "A4");
    }

    private void AcceptPayment()
    {
        if (selected is not { Stage: 3 }) return;
        if (MessageBox.Show($"{selected.Price:N0} so'm to'lov qabul qilindimi?", "To'lovni tasdiqlash", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        selected.Paid = true;
        int id = selected.Id; SaveOrders(); RefreshGrid(id);
    }

    private void SaveReceipt()
    {
        if (selected is not { Stage: 4 }) return;
        using var dialog = new SaveFileDialog { Filter = "Matnli chek (*.txt)|*.txt", FileName = $"Chek_{selected.Id}_{selected.Plate.Replace(' ', '_')}.txt" };
        if (dialog.ShowDialog() != DialogResult.OK) return;
        var text = new StringBuilder()
            .AppendLine("AVTO YUVISH — XIZMAT CHEKI (DEMO)")
            .AppendLine(new string('-', 35))
            .AppendLine($"Buyurtma: №{selected.Id}")
            .AppendLine($"Sana: {selected.CompletedAt:dd.MM.yyyy HH:mm}")
            .AppendLine($"Mijoz: {selected.Customer}")
            .AppendLine($"Telefon: {selected.Phone}")
            .AppendLine($"Avtomobil: {selected.Car}")
            .AppendLine($"Davlat raqami: {selected.Plate}")
            .AppendLine($"Xizmat: {selected.Service}")
            .AppendLine($"To'lov: {selected.Price:N0} so'm")
            .AppendLine("Holati: TO'LANGAN / TOPSHIRILGAN")
            .AppendLine("Eslatma: fiskal chek emas, o'quv loyihasi.")
            .ToString();
        File.WriteAllText(dialog.FileName, text, Encoding.UTF8);
        MessageBox.Show("Chek saqlandi.", "Muvaffaqiyatli");
    }

    private void LoadOrders()
    {
        try
        {
            string path = Path.Combine(dataDir, "orders.json");
            if (File.Exists(path)) orders.AddRange(JsonSerializer.Deserialize<List<CarOrder>>(File.ReadAllText(path)) ?? []);
        }
        catch (Exception ex) { MessageBox.Show("Saqlangan buyurtmalarni ochib bo'lmadi: " + ex.Message); }
    }

    private void SaveOrders()
    {
        try
        {
            Directory.CreateDirectory(dataDir);
            File.WriteAllText(Path.Combine(dataDir, "orders.json"), JsonSerializer.Serialize(orders, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex) { MessageBox.Show("Buyurtmalarni saqlashda xato: " + ex.Message); }
    }
}
