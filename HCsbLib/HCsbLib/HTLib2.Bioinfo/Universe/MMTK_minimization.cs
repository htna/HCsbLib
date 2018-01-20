/* Allocate and initialize Output variable descriptors */
/*
static PyTrajectoryVariable *
get_data_descriptors(PyArrayObject *configuration, PyArrayObject *gradients,
		     double *p_energy, double *norm,
		     double *box_size, int box_size_length)
{
  static PyTrajectoryVariable vars[6];
  if (vars != NULL) {
    vars[0].name = "potential_energy";
    vars[0].text = "Potential energy: %lf, ";
    vars[0].unit = energy_unit_name;
    vars[0].type = PyTrajectory_Scalar;
    vars[0].class = PyTrajectory_Energy;
    vars[0].value.dp = p_energy;
    vars[1].name = "gradient_norm";
    vars[1].text = "Gradient norm: %lf\n";
    vars[1].unit = energy_gradient_unit_name;
    vars[1].type = PyTrajectory_Scalar;
    vars[1].class = PyTrajectory_Energy;
    vars[1].value.dp = norm;
    vars[2].name = "configuration";
    vars[2].text = "Configuration:\n";
    vars[2].unit = length_unit_name;
    vars[2].type = PyTrajectory_ParticleVector;
    vars[2].class = PyTrajectory_Configuration;
    vars[2].value.array = configuration;
    vars[3].name = "gradients";
    vars[3].text = "Energy gradients:\n";
    vars[3].unit = energy_gradient_unit_name;
    vars[3].type = PyTrajectory_ParticleVector;
    vars[3].class = PyTrajectory_Gradients;
    vars[3].value.array = gradients;
    vars[4].name = NULL;
    if (box_size != NULL) {
      vars[4].name = "box_size";
      vars[4].text = "Box size:";
      vars[4].unit = length_unit_name;
      vars[4].type = PyTrajectory_BoxSize;
      vars[4].class = PyTrajectory_Configuration;
      vars[4].value.dp = box_size;
      vars[4].length = box_size_length;
      vars[5].name = NULL;
    }
  }
  return vars;
}
*/












/* Steepest descent minimizer */
/*
static PyObject *
steepestDescent(PyObject *dummy, PyObject *args)
{
  PyObject *universe;
  PyUniverseSpecObject *universe_spec;
  PyArrayObject *configuration;
  PyArrayObject *fixed;
  PyListObject *spec_list;
  PyFFEvaluatorObject *evaluator;
  PyTrajectoryOutputSpec *output;
  vector3 *x, *f;
  long *fix;
  int atoms, moving_atoms;
  int steps;
  double step_size, gradient_convergence;
  char *description;

  PyArrayObject *gradients;
  PyTrajectoryVariable *data_descriptors;
  energy_data p_energy;
  double norm, factor;
  double min_energy, min_norm;
  vector3 *min_configuration = NULL, *min_gradients = NULL;
  int i, j;

  // Parse and check arguments
  if (!PyArg_ParseTuple(args, "OO!O!O!iddO!s", &universe,
			&PyArray_Type, &configuration,
			&PyArray_Type, &fixed,
			&PyFFEvaluator_Type, &evaluator,
			&steps, &step_size, &gradient_convergence,
			&PyList_Type, &spec_list, &description))
    return NULL;
  universe_spec = (PyUniverseSpecObject *)
                   PyObject_GetAttrString(universe, "_spec");
  if (universe_spec == NULL)
    return NULL;

  // Create gradient array
  gradients = (PyArrayObject *)PyArray_Copy(configuration);
  if (gradients == NULL)
    return NULL;
  // Set some convenient variables
  atoms = configuration->dimensions[0];
  x = (vector3 *)configuration->data;
  f = (vector3 *)gradients->data;
  fix = (long *)fixed->data;

  moving_atoms = atoms;
  for (j = 0; j < atoms; j++)
    if (fix[j])
      moving_atoms--;

  // Prepare output data descriptors
  data_descriptors = get_data_descriptors(configuration, gradients,
					  &p_energy.energy, &norm,
					  universe_spec->geometry_data,
					  universe_spec->geometry_data_length);

  // Allocate arrays to keep track of current best point
  min_configuration = (vector3 *)malloc(atoms*sizeof(vector3));
  min_gradients = (vector3 *)malloc(atoms*sizeof(vector3));
  if (min_configuration == NULL || min_gradients == NULL) {
    PyErr_SetString(PyExc_MemoryError, "");
    goto error2;
  }

  // Initialize output
  output = PyTrajectory_OutputSpecification(universe, spec_list,
					    description,
					    data_descriptors);
  if (output == NULL)
    goto error2;

  // Get write access for the minimization, switching to
  // read access only during energy evaluation
  PyUniverseSpec_StateLock(universe_spec, -1);

  // Minimization main loop
  p_energy.gradients = (PyObject *)gradients;
  p_energy.gradient_fn = NULL;
  p_energy.force_constants = NULL;
  p_energy.fc_fn = NULL;
  min_energy = 0.; // initialize to make gcc happy
  min_norm = 0.;   // initialize to make gcc happy
  for (i = 0; i < steps; i++) {
    PyUniverseSpec_StateLock(universe_spec, -2);
    PyUniverseSpec_StateLock(universe_spec, 1);
    (*evaluator->eval_func)(evaluator, &p_energy, configuration, i > 0);
    PyUniverseSpec_StateLock(universe_spec, 2);
    if (p_energy.error) {
      goto error;
    }
    PyUniverseSpec_StateLock(universe_spec, -1);
    norm = 0.;
    for (j = 0; j < atoms; j++)
      if (!fix[j])
	norm += f[j][0]*f[j][0] + f[j][1]*f[j][1] + f[j][2]*f[j][2];
    norm = sqrt(norm/moving_atoms);
    if (i == 0 || p_energy.energy < min_energy) {
      min_energy = p_energy.energy;
      min_norm = norm;
      copy_vectors(x, min_configuration, atoms);
      copy_vectors(f, min_gradients, atoms);
      step_size *= 1.1;
    }
    else {
      p_energy.energy = min_energy;
      norm = min_norm;
      copy_vectors(min_configuration, x, atoms);
      copy_vectors(min_gradients, f, atoms);
      step_size *= 0.5;
    }
    if (norm < gradient_convergence)
      break;
    if (PyTrajectory_Output(output, i, data_descriptors,
			    &evaluator->tstate_save) == -1) {
      PyUniverseSpec_StateLock(universe_spec, -2);
      goto error;
    }
    factor = step_size/norm;
    for (j = 0; j < atoms; j++)
      if (!fix[j]) {
	x[j][0] -= factor*f[j][0];
	x[j][1] -= factor*f[j][1];
	x[j][2] -= factor*f[j][2];
      }
    universe_spec->correction_function(x, atoms, universe_spec->geometry_data);
  }

  // Restore minimum and do output
  p_energy.energy = min_energy;
  norm = min_norm;
  copy_vectors(min_configuration, x, atoms);
  copy_vectors(min_gradients, f, atoms);
  if (PyTrajectory_Output(output, i, data_descriptors,
			  &evaluator->tstate_save) == -1) {
    PyUniverseSpec_StateLock(universe_spec, -2);
    goto error;
  }

  // Clean up and return None
  PyUniverseSpec_StateLock(universe_spec, -2);
  PyTrajectory_OutputFinish(output, i, 0, 1, data_descriptors);
  free(min_configuration);
  free(min_gradients);
  Py_DECREF(gradients);
  Py_INCREF(Py_None);
  return Py_None;

  // Clean up and return error
error:
  PyTrajectory_OutputFinish(output, i, 1, 1, data_descriptors);
error2:
  if (min_configuration != NULL)
    free(min_configuration);
  if (min_gradients != NULL)
    free(min_gradients);
  Py_DECREF(gradients);
  return NULL;
}
*/












/* Conjugate gradient minimizer */
/*
static PyObject *
conjugateGradient(PyObject *dummy, PyObject *args)
{
  PyObject *universe;
  PyUniverseSpecObject *universe_spec;
  PyArrayObject *configuration;
  PyArrayObject *fixed;
  PyListObject *spec_list;
  PyFFEvaluatorObject *evaluator;
  PyTrajectoryOutputSpec *output;
  vector3 *x, *f1, *f2, *h;
  long *fix;
  int atoms, moving_atoms;
  int steps;
  double step_size, gradient_convergence;
  char *description;

  PyArrayObject *gradients1, *gradients2, *direction;
  PyTrajectoryVariable *data_descriptors;
  energy_data p_energy;
  double norm_sq, last_norm_sq, dot, norm;
  double norm_h, line_convergence, sign, step;
  double last, a, b, ea, eb, da, db;
  int i, j, reset_count, niter;

  // Parse and check arguments
  if (!PyArg_ParseTuple(args, "OO!O!O!iddO!s", &universe,
			&PyArray_Type, &configuration,
			&PyArray_Type, &fixed,
			&PyFFEvaluator_Type, &evaluator,
			&steps, &step_size, &gradient_convergence,
			&PyList_Type, &spec_list, &description))
    return NULL;
  universe_spec = (PyUniverseSpecObject *)
                   PyObject_GetAttrString(universe, "_spec");
  if (universe_spec == NULL)
    return NULL;

  // Create gradient and direction arrays
  gradients1 = (PyArrayObject *)PyArray_Copy(configuration);
  if (gradients1 == NULL)
    return NULL;
  gradients2 = (PyArrayObject *)PyArray_Copy(configuration);
  if (gradients2 == NULL) {
    Py_DECREF(gradients1);
    return NULL;
  }
  direction = (PyArrayObject *)PyArray_Copy(configuration);
  if (direction == NULL) {
    Py_DECREF(gradients1);
    Py_DECREF(gradients2);
    return NULL;
  }
  // Set some convenient variables
  atoms = configuration->dimensions[0];
  x = (vector3 *)configuration->data;
  f1 = (vector3 *)gradients1->data;
  f2 = (vector3 *)gradients2->data;
  h = (vector3 *)direction->data;
  fix = (long *)fixed->data;

  moving_atoms = atoms;
  for (j = 0; j < atoms; j++)
    if (fix[j])
      moving_atoms--;

  // Prepare output data descriptors
  data_descriptors = get_data_descriptors(configuration, gradients1,
					  &p_energy.energy, &norm,
					  universe_spec->geometry_data,
					  universe_spec->geometry_data_length);

  // Initialize output
  output = PyTrajectory_OutputSpecification(universe, spec_list,
					    description,
					    data_descriptors);
  if (output == NULL)
    goto error2;

  // Minimization main loop
  reset_count = 0;
  p_energy.gradients = (PyObject *)gradients1;
  p_energy.gradient_fn = NULL;
  p_energy.force_constants = NULL;
  p_energy.fc_fn = NULL;
  PyUniverseSpec_StateLock(universe_spec, 1);
  (*evaluator->eval_func)(evaluator, &p_energy, configuration, 0);
  PyUniverseSpec_StateLock(universe_spec, 2);
  if (p_energy.error) {
    i = 0;
    goto error;
  }

  // Get write access for the minimization, switching to
  // read access only during energy evaluation
  PyUniverseSpec_StateLock(universe_spec, -1);

  norm_sq = 0.;
  for (i = 0; i < steps; i++) {
    last_norm_sq = norm_sq;
    norm_sq = 0.;
    for (j = 0; j < atoms; j++)
      if (!fix[j])
	norm_sq += f1[j][0]*f1[j][0] + f1[j][1]*f1[j][1] + f1[j][2]*f1[j][2];
    norm = sqrt(norm_sq/moving_atoms);
    if (norm < gradient_convergence)
      break;
    if (norm > 50.*gradient_convergence)
      reset_count++;
    if (PyTrajectory_Output(output, i, data_descriptors,
			    &evaluator->tstate_save) == -1) {
      PyUniverseSpec_StateLock(universe_spec, -2);
      goto error;
    }
    if (i == 0)
      copy_vectors(f1, h, atoms);
    else {
      dot = 0.;
      for (j = 0; j < atoms; j++)
	if (!fix[j]) {
	  dot += f1[j][0]*f2[j][0] + f1[j][1]*f2[j][1] + f1[j][2]*f2[j][2];
	  f2[j][0] = f1[j][0];
	  f2[j][1] = f1[j][1];
	  f2[j][2] = f1[j][2];
	}
      if (reset_count == 5*atoms) {
	for (j = 0; j < atoms; j++) {
	  h[j][0] = 0.;
	  h[j][1] = 0.;
	  h[j][2] = 0.;
	}
	reset_count = 0;
      }
      else
	scale_vectors(h, (norm_sq-dot)/last_norm_sq, atoms);
      add_vectors(f1, h, atoms);
    }
    line_convergence = 1.e-3*norm;
    if (line_convergence < gradient_convergence)
      line_convergence = gradient_convergence;
    // Line minimization
#define eval(p) \
    { \
      double d = (p)-last; \
      int j; \
      for (j = 0; j < atoms; j++) \
        if (!fix[j]) { \
	  x[j][0] += sign*d*h[j][0]; \
	  x[j][1] += sign*d*h[j][1]; \
	  x[j][2] += sign*d*h[j][2]; \
        } \
      PyUniverseSpec_StateLock(universe_spec, -2); \
      PyUniverseSpec_StateLock(universe_spec, 1); \
      (*evaluator->eval_func)(evaluator, &p_energy, configuration, 0); \
      PyUniverseSpec_StateLock(universe_spec, 2); \
      if (p_energy.error) { \
	PyEval_RestoreThread(evaluator->tstate_save); \
        goto error; \
      } \
      PyUniverseSpec_StateLock(universe_spec, -1); \
      dot = 0.; \
      for (j = 0; j < atoms; j++) \
        if (!fix[j]) \
	  dot += f1[j][0]*h[j][0] + f1[j][1]*h[j][1] + f1[j][2]*h[j][2]; \
      dot /= (sign*norm_h); \
      last = (p); \
    }

    norm_h = 0.;
    for (j = 0; j < atoms; j++)
      if (!fix[j])
	norm_h += h[j][0]*h[j][0] + h[j][1]*h[j][1] + h[j][2]*h[j][2];
    norm_h = sqrt(norm_h);
    dot = 0.;
    for (j = 0; j < atoms; j++)
      if (!fix[j])
	dot += f1[j][0]*h[j][0] + f1[j][1]*h[j][1] + f1[j][2]*h[j][2];
    sign = (dot > 0.) ? -1. : 1.;
    dot /= (sign*norm_h);
    step = step_size/norm_h;
    last = 0.;
    a = b = 0.;
    ea = eb = p_energy.energy;
    da = db = dot;
    niter = 0;
    while (1) {
      a = b; ea = eb; da = db;
      b += step;
      eval(b); eb = p_energy.energy; db = dot;
      if (db > 0)
	break;
      if (++niter == 100)
	break;
    }
    niter = 0;
    while (1) {
      double new = ((eb-ea)/norm_h+a*da-b*db)/(da-db);
      if (new < a || new > b)
	new = 0.5*(a+b);
      eval(new);
      if (dot < 0) {
	a = new; ea = p_energy.energy; da = dot;
      }
      else {
	b = new; eb = p_energy.energy; db = dot;
      }
      if (db < 0.01*line_convergence)
	break;
      if (++niter == 100)
	break;
    }
#undef eval
    universe_spec->correction_function(x, atoms, universe_spec->geometry_data);
  }

  // Final output
  if (PyTrajectory_Output(output, i, data_descriptors,
			  &evaluator->tstate_save) == -1)
      goto error;

  // Clean up and return None
  PyUniverseSpec_StateLock(universe_spec, -2);
  PyTrajectory_OutputFinish(output, i, 0, 1, data_descriptors);
  Py_DECREF(gradients1);
  Py_DECREF(gradients2);
  Py_INCREF(Py_None);
  return Py_None;

  // Clean up and return error
error:
  PyTrajectory_OutputFinish(output, i, 1, 1, data_descriptors);
error2:
  Py_DECREF(gradients1);
  Py_DECREF(gradients2);
  Py_DECREF(direction);
  return NULL;
}
*/
