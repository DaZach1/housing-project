import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ResidentService } from '../../services/resident.service';
import { ResidentDto } from '../../models/house-dto';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-resident-create',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './resident-create.component.html',
  styleUrls: ['./resident-create.component.css']
})
export class ResidentCreateComponent {
  newResident: ResidentDto = {
    id: 0,
    firstName: '',
    lastName: '',
    personalCode: '',
    dateOfBirth: new Date(),
    phone: '',
    email: '',
    password: '',
    apartmentId: 0,
    apartmentLink: '',
    isOwner: false,
    userId: '',
  };

  constructor(
    private residentService: ResidentService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    const apartmentId = this.route.snapshot.paramMap.get('apartmentId');
    if (apartmentId) {
      this.newResident.apartmentId = +apartmentId;
    } else {
      console.error('Apartment ID not found in URL');
      this.router.navigate(['/all-houses']);
    }
  }

  createResident(): void {
    this.residentService.createResident(this.newResident).subscribe({
      next: (createdResident) => {
        this.router.navigate(['/apartment', this.newResident.apartmentId]);
      },
      error: (err) => {
        console.error('Error creating resident:', err.error);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/apartment', this.newResident.apartmentId]);
  }
}