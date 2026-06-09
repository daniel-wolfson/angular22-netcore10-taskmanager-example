import { Component, computed, inject, output } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskItem } from '../../models/task-item.model';
import { CreateTaskRequest, TaskService } from '../../services/task.service';

@Component({
  selector: 'app-create-task',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './create-task.component.html',
  styleUrl: './create-task.component.css'
})
export class CreateTaskComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly taskService = inject(TaskService);

  readonly created = output<TaskItem>();
  readonly submitDisabled = computed(() => this.formStatus() !== 'VALID');

  // Initializes the reactive form with validation rules 
  // for the title and description fields
  readonly form = this.formBuilder.group({
    title: ['', [
      Validators.required, 
      Validators.minLength(3), 
      Validators.maxLength(100)]],
    description: ['']
  });

  // Converts the form's statusChanges observable into a signal to 
  // track the form's validation status
  private readonly formStatus = toSignal(this.form.statusChanges, {
    initialValue: this.form.status,
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    // create a new task
    const request = this.form.getRawValue() as CreateTaskRequest;
    this.taskService.createTask(request).subscribe((task) => {
      this.created.emit(task);
      this.form.reset({ title: '', description: '' });
      this.form.markAsPristine();
      this.form.markAsUntouched();
    });
  }
}
