import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApartmentService } from '../../services/apartment.service';
import { ResidentService } from '../../services/resident.service';
import { ApartmentDto } from '../../models/house-dto';
import {HasRoleDirective} from '../../role.directive';

@Component({
  selector: 'app-apartment-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, HasRoleDirective],
  templateUrl: './apartment-detail.component.html',
  styleUrls: ['./apartment-detail.component.css']
})
export class ApartmentDetailComponent implements OnInit {
  apartment: ApartmentDto | null = null;
  isEditMode = false;
  isLoading = true;
  error: string | null = null;
  editedApartment: Partial<ApartmentDto> = {};
  showDeleteModal = false;
  residentToDelete: number | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apartmentService: ApartmentService,
    private residentService: ResidentService
  ) {}

  ngOnInit(): void {
    this.loadApartment();
  }

  loadApartment(): void {
    
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error = 'Invalid apartment ID';
      this.isLoading = false;
      return;
    }

    this.apartmentService.getApartmentById(+id).subscribe({
      next: (apartment) => {
        this.apartment = apartment;
        this.editedApartment = { ...apartment };
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading apartment:', err);
        this.error = 'Failed to load apartment details';
        this.isLoading = false;
      }
    });
  }

  openDeleteModal(residentId: number): void {
    this.residentToDelete = residentId;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.residentToDelete = null;
  }

  confirmDelete(): void {
    if (this.residentToDelete && this.apartment) {
      this.residentService.deleteResident(this.residentToDelete).subscribe({
        next: () => {
          this.loadApartment();
          this.closeDeleteModal();
        },
        error: (err) => {
          console.error('Error deleting resident:', err);
          this.closeDeleteModal();
        }
      });
    }
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
    if (!this.isEditMode && this.apartment) {
      this.editedApartment = { ...this.apartment };
    }
  }

  saveChanges(): void {
    if (!this.apartment || !this.editedApartment) {
      console.error('Apartment data is not loaded');
      return;
    }

    this.isLoading = true;
    this.error = null;

    this.apartmentService.updateApartment(this.apartment.id, this.editedApartment as ApartmentDto).subscribe({
      next: (updatedApartment) => {
        this.loadApartment();
        
        this.apartment = updatedApartment;
        this.isEditMode = false;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error updating apartment:', err);
        this.error = 'Failed to update apartment details';
        this.isLoading = false;
      }
    });
  }

  navigateToResident(residentId: number): void {
    this.router.navigate(['/resident', residentId]);
  }
}