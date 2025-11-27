using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAsetPemda.Data.Models;

/// <summary>
/// Asset import data table - matches columns from EXCELL 7 KECAMATAN KOTAPINANG.xls
/// This table holds raw imported data from Excel files before processing/validation
/// </summary>
public class AssetImportData
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Urut - Sequential number from Excel
    /// </summary>
    public int? Urut { get; set; }

    /// <summary>
    /// Kd_UPB - Kode UPB (Unit Pengguna Barang Code)
    /// </summary>
    [MaxLength(50)]
    public string? Kd_UPB { get; set; }

    /// <summary>
    /// Nm_UPB - Nama UPB (Unit Pengguna Barang Name)
    /// </summary>
    [MaxLength(255)]
    public string? Nm_UPB { get; set; }

    /// <summary>
    /// No_Ruang - Nomor Ruangan (Room Number)
    /// </summary>
    [MaxLength(50)]
    public string? No_Ruang { get; set; }

    /// <summary>
    /// Nm_Ruang - Nama Ruangan (Room Name)
    /// </summary>
    [MaxLength(255)]
    public string? Nm_Ruang { get; set; }

    /// <summary>
    /// Jns_Aset - Jenis Aset (Asset Type)
    /// 1. Tanah, 2. Peralatan dan Mesin, 3. Gedung dan Bangunan, etc.
    /// </summary>
    [MaxLength(100)]
    public string? Jns_Aset { get; set; }

    /// <summary>
    /// Kode_Barang - Kode Barang (Asset Code)
    /// </summary>
    [MaxLength(100)]
    public string? Kode_Barang { get; set; }

    /// <summary>
    /// Nm_Barang - Nama Barang (Asset Name)
    /// </summary>
    [MaxLength(500)]
    public string? Nm_Barang { get; set; }

    /// <summary>
    /// Jn_Kap - Jenis Kapitalisasi (Capitalization Type: BARU/REHAB)
    /// </summary>
    [MaxLength(50)]
    public string? Jn_Kap { get; set; }

    /// <summary>
    /// No_Kap - Nomor Kapitalisasi (Capitalization Document Number)
    /// </summary>
    [MaxLength(100)]
    public string? No_Kap { get; set; }

    /// <summary>
    /// Reg_Kap - Register Kapitalisasi (Parent Asset Register for Capitalization)
    /// </summary>
    [MaxLength(100)]
    public string? Reg_Kap { get; set; }

    /// <summary>
    /// Tgl_Perolehan - Tanggal Perolehan (Acquisition Date)
    /// </summary>
    public DateTime? Tgl_Perolehan { get; set; }

    /// <summary>
    /// Keterangan - Notes/Remarks
    /// </summary>
    [MaxLength(1000)]
    public string? Keterangan { get; set; }

    /// <summary>
    /// Kd_Posisi - Kode Posisi (Position Code)
    /// </summary>
    [MaxLength(50)]
    public string? Kd_Posisi { get; set; }

    /// <summary>
    /// Pemilik - Owner
    /// </summary>
    [MaxLength(255)]
    public string? Pemilik { get; set; }

    /// <summary>
    /// Asal_Usul - Origin (Pembelian, Hibah, etc.)
    /// </summary>
    [MaxLength(100)]
    public string? Asal_Usul { get; set; }

    /// <summary>
    /// Kon_b - Kondisi Barang (Asset Condition: 2, 3, 4, 5)
    /// </summary>
    [MaxLength(10)]
    public string? Kon_b { get; set; }

    /// <summary>
    /// Jl_Barang - Jumlah Barang (Quantity)
    /// </summary>
    public int? Jl_Barang { get; set; }

    /// <summary>
    /// Harga - Harga per Unit (Price per Unit)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Harga { get; set; }

    /// <summary>
    /// Harga_Total - Total Price
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Harga_Total { get; set; }

    /// <summary>
    /// Masa_Manfaat - Masa Manfaat Penambahan (Additional Useful Life)
    /// </summary>
    public int? Masa_Manfaat { get; set; }

    /// <summary>
    /// KDP - Konstruksi Dalam Pengerjaan (Ya/Tidak)
    /// </summary>
    [MaxLength(10)]
    public string? KDP { get; set; }

    /// <summary>
    /// Intra_Ekstra - Intrakomptabel/Ekstrakomptabel
    /// </summary>
    [MaxLength(50)]
    public string? Intra_Ekstra { get; set; }

    /// <summary>
    /// Alamat - Address (for Tanah, Gedung, Jalan)
    /// </summary>
    [MaxLength(500)]
    public string? Alamat { get; set; }

    /// <summary>
    /// Panjang - Length (in meters)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Panjang { get; set; }

    /// <summary>
    /// Lebar - Width (in meters)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Lebar { get; set; }

    /// <summary>
    /// Luas - Area (in m²) for Tanah, Gedung, Jalan
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Luas { get; set; }

    /// <summary>
    /// Hak_Tanah - Land Rights (for Tanah)
    /// </summary>
    [MaxLength(100)]
    public string? Hak_Tanah { get; set; }

    /// <summary>
    /// Penggunaan_Merk - Usage (Tanah) / Merk (Peralatan)
    /// </summary>
    [MaxLength(255)]
    public string? Penggunaan_Merk { get; set; }

    /// <summary>
    /// Type_Panjang - Type (Peralatan) / Panjang (Aset Lainnya)
    /// </summary>
    [MaxLength(255)]
    public string? Type_Panjang { get; set; }

    /// <summary>
    /// CC_Lebar - CC (Peralatan) / Lebar (Aset Lainnya)
    /// </summary>
    [MaxLength(255)]
    public string? CC_Lebar { get; set; }

    /// <summary>
    /// Bahan - Material (Peralatan, Aset Lainnya)
    /// </summary>
    [MaxLength(255)]
    public string? Bahan { get; set; }

    /// <summary>
    /// Tgl_Dokumen - Document Date (Sertifikat/Kontrak)
    /// </summary>
    public DateTime? Tgl_Dokumen { get; set; }

    /// <summary>
    /// No_Dokumen - Document Number (Sertifikat/Kontrak)
    /// </summary>
    [MaxLength(100)]
    public string? No_Dokumen { get; set; }

    /// <summary>
    /// Status_Tanah - Land Status (for Gedung, Jalan)
    /// </summary>
    [MaxLength(100)]
    public string? Status_Tanah { get; set; }

    /// <summary>
    /// Bertingkat - Multi-story building (Ya/Tidak)
    /// </summary>
    [MaxLength(10)]
    public string? Bertingkat { get; set; }

    /// <summary>
    /// Beton - Concrete construction (Ya/Tidak)
    /// </summary>
    [MaxLength(10)]
    public string? Beton { get; set; }

    /// <summary>
    /// No_Rangka - Chassis Number (for Vehicles)
    /// </summary>
    [MaxLength(100)]
    public string? No_Rangka { get; set; }

    /// <summary>
    /// No_Mesin - Engine Number (for Vehicles)
    /// </summary>
    [MaxLength(100)]
    public string? No_Mesin { get; set; }

    /// <summary>
    /// No_Polisi - License Plate Number (for Vehicles)
    /// </summary>
    [MaxLength(50)]
    public string? No_Polisi { get; set; }

    /// <summary>
    /// No_BPKB - Vehicle Registration Certificate Number
    /// </summary>
    [MaxLength(100)]
    public string? No_BPKB { get; set; }

    /// <summary>
    /// Nm_File - File Name (reference to supporting documents)
    /// </summary>
    [MaxLength(500)]
    public string? Nm_File { get; set; }

    /// <summary>
    /// Proc_Id - Process ID from Excel
    /// </summary>
    [MaxLength(100)]
    public string? Proc_Id { get; set; }

    /// <summary>
    /// Import batch reference - to track which import session this record belongs to
    /// </summary>
    public Guid? ImportBatchId { get; set; }

    /// <summary>
    /// Source file name
    /// </summary>
    [MaxLength(255)]
    public string? SourceFileName { get; set; }

    /// <summary>
    /// Import timestamp
    /// </summary>
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User who imported this data
    /// </summary>
    [MaxLength(255)]
    public string? ImportedBy { get; set; }

    /// <summary>
    /// Has this record been processed/validated
    /// </summary>
    public bool IsProcessed { get; set; } = false;

    /// <summary>
    /// Validation errors (if any)
    /// </summary>
    [MaxLength(2000)]
    public string? ValidationErrors { get; set; }
}
