import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ResidentService } from '../../services/resident.service';
import { ResidentDto } from '../../models/house-dto';

@Component({
  selector: 'app-resident-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './resident-detail.component.html',
  styleUrls: ['./resident-detail.component.css']
})
export class ResidentDetailComponent implements OnInit {
  resident: ResidentDto | null = null;
  isEditMode = false;
  isLoading = true;
  error: string | null = null;
  editedResident: Partial<ResidentDto> = {};

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private residentService: ResidentService
  ) {}

  ngOnInit(): void {
    this.loadResident();
  }

  loadResident(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error = 'Invalid resident ID';
      this.isLoading = false;
      return;
    }

    this.residentService.getResidentById(+id).subscribe({
      next: (resident) => {
        this.resident = resident;
        this.editedResident = { ...resident };
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading resident:', err);
        this.error = 'Failed to load resident details';
        this.isLoading = false;
      }
    });
  }

  toggleEditMode(): void {
    this.isEditMode = !this.isEditMode;
    if (!this.isEditMode) {
      this.editedResident = this.resident ? { ...this.resident } : {};
    }
  }

  saveChanges(): void {
    if (!this.resident || !this.editedResident) {
      console.error('Resident data is not loaded');
      return;
    }
    console.log('Sending data:', this.editedResident);
    this.isLoading = true;
    this.error = null;
    

    this.residentService.updateResident(this.resident.id, this.editedResident as ResidentDto).subscribe({
      next: (updatedResident) => {
        this.loadResident();
        this.resident = updatedResident;
        this.isEditMode = false;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Update error:', err);
        this.error = err.message || 'Failed to update resident';
        this.isLoading = false;
      }
    });
  }
}