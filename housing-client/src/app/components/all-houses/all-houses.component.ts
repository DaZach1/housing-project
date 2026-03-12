import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { HouseService } from '../../services/house.service';
import { HouseDto } from '../../models/house-dto';
import { HasRoleDirective } from '../../role.directive';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-all-houses',
  standalone: true,
  imports: [CommonModule, RouterModule, HasRoleDirective],
  templateUrl: './all-houses.component.html',
  styleUrls: ['./all-houses.component.css']
})
export class AllHousesComponent implements OnInit {
  houses: HouseDto[] = [];
  isLoading = true;
  showDeleteModal = false;
  houseToDelete: number | null = null;

  constructor(
    private houseService: HouseService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadHouses();
  }

  navigateToLogin(): void {
    // Полная очистка данных перед переходом
    this.authService.logout().subscribe({
      complete: () => {
        // Принудительная перезагрузка страницы
        window.location.href = '/login';
      }
    });
  }

  openDeleteModal(houseId: number): void {
    this.houseToDelete = houseId;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.houseToDelete = null;
  }

  confirmDelete(): void {
    if (this.houseToDelete) {
      this.houseService.deleteHouse(this.houseToDelete).subscribe({
        next: () => {
          this.loadHouses();
          this.closeDeleteModal();
        },
        error: (err) => {
          console.error('Error deleting house:', err);
          this.closeDeleteModal();
        }
      });
    }
  }

  loadHouses(): void {
    this.houseService.getHouses().subscribe({
      next: (houses: HouseDto[]) => {
        this.houses = houses;
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Error loading houses', err);
        this.isLoading = false;
      }
    });
  }
}