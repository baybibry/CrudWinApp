using crudapp.services;
using crudApp.model;

namespace CrudWinApp;

public partial class Form1 : Form
{
    private readonly ProductService _productService = new ProductService();

    // ── Controls ──────────────────────────────────────────────────────────────
    private int _selectedId = 0;

    private DataGridView dgvProducts;
    private TextBox txtName, txtDescription, txtPrice, txtCostPrice, txtDiscount, txtQuantity;
    private CheckBox chkAvailable;
    private Button btnLoad, btnAdd, btnUpdate, btnDelete, btnClear;
    private Label lblName, lblDescription, lblPrice, lblCostPrice, lblDiscount, lblQuantity, lblAvailable;
    private GroupBox grpForm;

    public Form1()
    {
        InitializeComponent();
        BuildUI();
        LoadProducts();
    }

    // ── UI Builder ────────────────────────────────────────────────────────────
    private void BuildUI()
    {
        this.Text          = "Product Management";
        this.Width         = 860;
        this.Height        = 600;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox   = false;

        // DataGridView
        dgvProducts = new DataGridView
        {
            Location          = new Point(10, 10),
            Size              = new Size(820, 250),
            ReadOnly          = true,
            SelectionMode     = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect       = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows    = false,
            AllowUserToDeleteRows = false,
        };
        dgvProducts.SelectionChanged += DgvProducts_SelectionChanged;

        // Group box for the form fields
        grpForm = new GroupBox
        {
            Text     = "Product Details",
            Location = new Point(10, 270),
            Size     = new Size(820, 240),
        };

        // Labels & TextBoxes
        int col1X = 15, col2X = 270, col3X = 530;
        int row1 = 25, row2 = 70, row3 = 115, row4 = 160;

        lblName        = MakeLabel("Product Name:", col1X, row1);
        txtName        = MakeTextBox(col1X,         row1 + 20, 220);

        lblDescription = MakeLabel("Description:",  col2X, row1);
        txtDescription = MakeTextBox(col2X,         row1 + 20, 220);

        lblPrice       = MakeLabel("Price:",        col2X, row2);
        txtPrice       = MakeTextBox(col2X,         row2 + 20, 100);

        lblCostPrice   = MakeLabel("Cost Price:",   col3X, row1);
        txtCostPrice   = MakeTextBox(col3X,         row1 + 20, 100);

        lblDiscount    = MakeLabel("Discount:",     col3X, row2);
        txtDiscount    = MakeTextBox(col3X,         row2 + 20, 100);

        lblQuantity    = MakeLabel("Quantity:",     col1X, row3);
        txtQuantity    = MakeTextBox(col1X,         row3 + 20, 100);

        lblAvailable   = MakeLabel("Available:",    col2X, row3);
        chkAvailable   = new CheckBox
        {
            Location = new Point(col2X, row3 + 20),
            Size     = new Size(20, 20),
        };

        // Buttons
        btnLoad   = MakeButton("Load",   col1X,        row4, Color.LightGray);
        btnAdd    = MakeButton("Add",    col1X + 90,   row4, Color.LightGreen);
        btnUpdate = MakeButton("Update", col1X + 180,  row4, Color.LightYellow);
        btnDelete = MakeButton("Delete", col1X + 270,  row4, Color.LightCoral);
        btnClear  = MakeButton("Clear",  col1X + 360,  row4, Color.LightGray);

        btnLoad.Click   += async (s, e) => await LoadProducts();
        btnAdd.Click    += async (s, e) => await AddProduct();
        btnUpdate.Click += async (s, e) => await UpdateProduct();
        btnDelete.Click += async (s, e) => await DeleteProduct();
        btnClear.Click  += (s, e) => ClearForm();

        grpForm.Controls.AddRange(new Control[]
        {
            lblName, txtName,
            lblDescription, txtDescription,
            lblPrice, txtPrice,
            lblCostPrice, txtCostPrice,
            lblDiscount, txtDiscount,
            lblQuantity, txtQuantity,
            lblAvailable, chkAvailable,
            btnLoad, btnAdd, btnUpdate, btnDelete, btnClear,
        });

        this.Controls.Add(dgvProducts);
        this.Controls.Add(grpForm);
    }

    // ── CRUD Operations ───────────────────────────────────────────────────────
    private async Task LoadProducts()
    {
        try
        {
            var products = await _productService.GetAllProducts();
            dgvProducts.DataSource = products;
        }
        catch (Exception ex)
        {
            if (ex.Message == "No products found.")
                dgvProducts.DataSource = null;
            else
                MessageBox.Show("Error loading products:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task AddProduct()
    {
        if (!ValidateForm()) return;

        var product = GetFormProduct();
        try
        {
            await _productService.AddProduct(product);
            MessageBox.Show("Product added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            await LoadProducts();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error adding product:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task UpdateProduct()
    {
        if (_selectedId == 0)
        {
            MessageBox.Show("Select a product to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        int id = _selectedId;
        if (!ValidateForm()) return;

        var product = GetFormProduct();
        try
        {
            await _productService.UpdateProduct(id, product);
            MessageBox.Show("Product updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            await LoadProducts();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error updating product:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task DeleteProduct()
    {
        if (_selectedId == 0)
        {
            MessageBox.Show("Select a product to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        int id = _selectedId;

        var confirm = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes) return;

        try
        {
            await _productService.DeleteProduct(id);
            MessageBox.Show("Product deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
            await LoadProducts();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error deleting product:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ── Grid Selection ────────────────────────────────────────────────────────
    private void DgvProducts_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvProducts.SelectedRows.Count == 0) return;

        var row = dgvProducts.SelectedRows[0];
        _selectedId         = Convert.ToInt32(row.Cells["Id"]?.Value ?? 0);
        txtName.Text        = row.Cells["ProductName"]?.Value?.ToString() ?? "";
        txtDescription.Text = row.Cells["Description"]?.Value?.ToString() ?? "";
        txtPrice.Text       = row.Cells["Price"]?.Value?.ToString() ?? "";
        txtCostPrice.Text   = row.Cells["ConstPrice"]?.Value?.ToString() ?? "";
        txtDiscount.Text    = row.Cells["Discount"]?.Value?.ToString() ?? "";
        txtQuantity.Text    = row.Cells["Quantity"]?.Value?.ToString() ?? "";
        chkAvailable.Checked = row.Cells["IsAvailable"]?.Value is true;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))        { MessageBox.Show("Product Name is required.", "Validation"); return false; }
        if (!decimal.TryParse(txtPrice.Text, out _))        { MessageBox.Show("Price must be a number.", "Validation"); return false; }
        if (!decimal.TryParse(txtCostPrice.Text, out _))    { MessageBox.Show("Cost Price must be a number.", "Validation"); return false; }
        if (!decimal.TryParse(txtDiscount.Text, out _))     { MessageBox.Show("Discount must be a number.", "Validation"); return false; }
        if (!long.TryParse(txtQuantity.Text, out _))        { MessageBox.Show("Quantity must be a whole number.", "Validation"); return false; }
        return true;
    }

    private ProductModel GetFormProduct() => new ProductModel
    {
        ProductName = txtName.Text.Trim(),
        Description = txtDescription.Text.Trim(),
        Price       = decimal.Parse(txtPrice.Text),
        ConstPrice  = decimal.Parse(txtCostPrice.Text),
        Discount    = decimal.Parse(txtDiscount.Text),
        Quantity    = long.Parse(txtQuantity.Text),
        IsAvailable = chkAvailable.Checked,
    };

    private void ClearForm()
    {
        _selectedId = 0;
        txtName.Text = txtDescription.Text = "";
        txtPrice.Text = txtCostPrice.Text = txtDiscount.Text = txtQuantity.Text = "";
        chkAvailable.Checked = false;
        dgvProducts.ClearSelection();
    }

    private static Label MakeLabel(string text, int x, int y) => new Label
    {
        Text     = text,
        Location = new Point(x, y),
        AutoSize = true,
    };

    private static TextBox MakeTextBox(int x, int y, int width, bool readOnly = false) => new TextBox
    {
        Location = new Point(x, y),
        Width    = width,
        ReadOnly = readOnly,
    };

    private static Button MakeButton(string text, int x, int y, Color backColor) => new Button
    {
        Text      = text,
        Location  = new Point(x, y),
        Width     = 80,
        Height    = 28,
        BackColor = backColor,
    };
}
