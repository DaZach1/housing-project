import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HouseService } from '../../services/house.service';
import { HouseDto } from '../../models/house-dto';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-house-create',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './house-create.component.html',
  styleUrls: ['./house-create.component.css']
})
export class HouseCreateComponent {
  newHouse: HouseDto = {
    id: 0,
    number: 0,
    street: '',
    city: '',
    country: '',
    postalCode: '',
    apartments: []
  };

  constructor(
    private houseService: HouseService,
    private router: Router
  ) {}

  createHouse(): void {
    this.houseService.createHouse(this.newHouse).subscribe({
      next: (createdHouse) => {
        this.router.navigate(['/house', createdHouse.id]);
      },
      error: (err) => {
        console.error('Error creating house:', err);
      }
    });
  }
  cancel(): void {
    this.router.navigate(['/all-houses']); }
  
}