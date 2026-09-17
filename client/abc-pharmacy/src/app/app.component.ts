import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { PharmacyService } from './pharmacy.service';
import { CreateMedicine, Medicine, SaleRecord } from './models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  private readonly pharmacy = inject(PharmacyService);
  private readonly fb = inject(FormBuilder);

  medicines: Medicine[] = [];
  sales: SaleRecord[] = [];
  search = '';
  message = '';
  error = '';
  activeTab: 'medicines' | 'sales' = 'medicines';

  medicineForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    notes: [''],
    expiryDate: ['', Validators.required],
    quantity: [0, [Validators.required, Validators.min(0)]],
    price: [0, [Validators.required, Validators.min(0)]],
    brand: ['', [Validators.required, Validators.maxLength(100)]]
  });

  saleForm = this.fb.nonNullable.group({
    medicineId: [0, Validators.min(1)],
    quantity: [1, [Validators.required, Validators.min(1)]],
    soldBy: ['admin']
  });

  constructor() {
    this.loadMedicines();
  }

  loadMedicines(): void {
    this.clearStatus();
    this.pharmacy.getMedicines(this.search).subscribe({
      next: data => this.medicines = data,
      error: err => this.showError(err)
    });
  }

  addMedicine(): void {
    if (this.medicineForm.invalid) {
      this.medicineForm.markAllAsTouched();
      return;
    }

    const value = this.medicineForm.getRawValue() as CreateMedicine;
    this.pharmacy.addMedicine(value).subscribe({
      next: () => {
        this.message = 'Medicine added successfully.';
        this.medicineForm.reset({
          fullName: '', notes: '', expiryDate: '', quantity: 0, price: 0, brand: ''
        });
        this.loadMedicines();
      },
      error: err => this.showError(err)
    });
  }

  recordSale(): void {
    if (this.saleForm.invalid) {
      this.saleForm.markAllAsTouched();
      return;
    }

    const value = this.saleForm.getRawValue();
    this.pharmacy.sellMedicine(value.medicineId, value.quantity, value.soldBy).subscribe({
      next: sale => {
        this.message = `Sale recorded. Total amount: ₹${sale.totalAmount.toFixed(2)}`;
        this.loadMedicines();
        this.loadSales();
      },
      error: err => this.showError(err)
    });
  }

  loadSales(): void {
    this.pharmacy.getSales().subscribe({
      next: data => this.sales = data,
      error: err => this.showError(err)
    });
  }

  selectMedicineForSale(id: number): void {
    this.saleForm.patchValue({ medicineId: id, quantity: 1 });
    this.activeTab = 'sales';
  }

  isExpiryWarning(expiryDate: string): boolean {
    const expiry = new Date(expiryDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const diffDays = Math.ceil((expiry.getTime() - today.getTime()) / 86400000);
    return diffDays <= 30;
  }

  isLowStock(quantity: number): boolean {
    return quantity < 10;
  }

  rowClass(medicine: Medicine): string {
    // Expiry has priority if both rules apply.
    if (this.isExpiryWarning(medicine.expiryDate)) return 'expiry-row';
    if (this.isLowStock(medicine.quantity)) return 'low-stock-row';
    return '';
  }

  private clearStatus(): void {
    this.message = '';
    this.error = '';
  }

  private showError(err: unknown): void {
    const response = err as HttpErrorResponse;
    this.error = response?.error?.message ?? 'An unexpected error occurred.';
    this.message = '';
  }
}
