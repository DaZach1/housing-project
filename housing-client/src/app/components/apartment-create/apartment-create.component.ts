import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApartmentService } from '../../services/apartment.service';
import { ApartmentDto } from '../../models/house-dto';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-apartment-create',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './apartment-create.component.html',
  styleUrls: ['./apartment-create.component.css']
})
export class ApartmentCreateComponent {
  newApartment: ApartmentDto = {
    id: 0,
    number: 0,
    floor: 0,
    roomCount: 0,
    population: 0,
    totalArea: 0,
    livingArea: 0,
    houseId: 0, 
    houseLink: '',
    residents: []
  };

  constructor(
    private apartmentService: ApartmentService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    // Получаем houseId из URL
    const houseId = this.route.snapshot.paramMap.get('houseId');
    if (houseId) {
      this.newApartment.houseId = +houseId;
    } else {
      console.error('House ID not found in URL');
      this.router.navigate(['/all-houses']);
    }
  }

  createApartment(): void {
    this.apartmentService.createApartment(this.newApartment).subscribe({
      next: (createdApartment) => {
        // Перенаправляем на страницу дома после создания
        this.router.navigate(['/house', this.newApartment.houseId]);
      },
      error: (err) => {
        console.error('Error creating apartment:', err);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/house', this.newApartment.houseId]);
  }
}