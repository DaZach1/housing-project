import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HouseService } from '../../services/house.service';
import { HouseDto } from '../../models/house-dto';
import { ApartmentService } from '../../services/apartment.service';
import {HasRoleDirective} from '../../role.directive';

@Component({
  selector: 'app-house-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, HasRoleDirective],
  templateUrl: './house-detail.component.html',
  styleUrls: ['./house-detail.component.css']
})
export class HouseDetailComponent implements OnInit {
  house: HouseDto | null = null;
  isEditMode = false;
  isLoading = true;
  error: string | null = null;
  editedHouse: Partial<HouseDto> = {};
  showDeleteModal = false;
  apartmentToDelete: number | null = null;
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private houseService: HouseService,
    private apartmentService: ApartmentService
  ) {}

  ngOnInit(): void {
    this.loadHouse();
  }
  openDeleteModal(houseId: number): void {
    this.apartmentToDelete = houseId;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.apartmentToDelete = null;
  }

  confirmDelete(): void {
    if (this.apartmentToDelete) {
      this.apartmentService.deleteApartment(this.apartmentToDelete).subscribe({
        next: () => {
          this.loadHouse();
          this.closeDeleteModal();
        },
        error: (err) => {
          console.error('Error deleting house:', err);
          this.closeDeleteModal();
        }
      });
    }
  }
  loadHouse(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error = 'Invalid house ID';
      this.isLoading = false;
      return;
    }

    this.houseService.getHouseById(+id).subscribe({
      next: (house) => {
        this.house = house;
        this.editedHouse = { ...house };
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading house:', err);
        this.error = 'Failed to load house details';
        this.isLoading = false;
      }
    });
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
    if (!this.isEditMode) {
      this.editedHouse = this.house ? { ...this.house } : {};
    }
  }
  
  saveChanges(): void {
    if (!this.house || !this.editedHouse) {
      console.error('House data is not loaded');
      return;
    }
  
    this.isLoading = true;
    this.error = null;
  
    this.houseService.updateHouse(this.house.id, this.editedHouse as HouseDto).subscribe({
      next: (updatedHouse) => {
        this.loadHouse();

        this.house = updatedHouse;
        this.isEditMode = false;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Update error:', err);
        this.error = err.message || 'Failed to update house';
        this.isLoading = false;
      }
    });
  }

  navigateToApartment(apartmentId: number): void {
    this.router.navigate(['/apartment', apartmentId]);
  }
}