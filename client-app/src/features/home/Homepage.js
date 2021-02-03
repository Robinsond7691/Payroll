import React, { Fragment } from 'react';
import {
  Button,
  Divider,
  Grid,
  Header,
  Image,
  Search,
  Segment,
} from 'semantic-ui-react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';
import { history } from '../..';

import { Jobsites } from '../../app/api/agent';
import LoginForm from '../user/LoginForm';
import {
  useAuthDispatch,
  useAuthState,
} from '../../app/context/auth/authContext';
import { useModalDispatch } from '../../app/context/modal/modalContext';
import { logoutUser } from '../../app/context/auth/authActions';
import { openModal } from '../../app/context/modal/modalActions';
import { clockInUser, clockOutUser } from '../../app/context/auth/authActions';

const TestHome = () => {
  const { isAuthenticated, user } = useAuthState();
  const authDispatch = useAuthDispatch();
  const modalDispatch = useModalDispatch();

  const [results, setResults] = React.useState([]);
  const [loading, setLoading] = React.useState(false);
  const [selectionValue, setSelectionValue] = React.useState('');

  //handle selection of search result
  const handleResultSelect = (e, { result }) => setSelectionValue(result.title);

  //handler for jobsite search bar
  const handleSearchChange = async (e, { value }) => {
    setSelectionValue(value);
    setLoading(true);
    let searchResults = await Jobsites.listJobsites(value, 4, 1);
    searchResults = searchResults.data;

    //create a list of objects that the search will accept
    let newResults = [];
    searchResults.forEach((jobsite) => {
      newResults.push({
        title: jobsite.moniker,
        description: `${jobsite.name} - ${jobsite.location.cityTown}`,
      });
    });
    setLoading(false);
    setResults(newResults);
  };

  //Function that prints date of clockedIn timestamp
  const showClockedInDate = () => {
    const timestamp = new Date(user.clockedInTimestamp.clockedIn);
    const date = format(timestamp, 'eeee, MMMM do, yyyy');
    const time = format(timestamp, 'h:mm a');

    return `${date} at ${time}`;
  };

  //if a manager logs in, redirect
  React.useEffect(() => {
    if (user && user.manager === true) history.push('/dashboard');
  }, [user]);

  return (
    <Segment inverted textAlign='center' vertical className='masthead'>
      <Grid
        textAlign='center'
        style={{ margin: 'auto' }}
        verticalAlign='middle'
      >
        <Grid.Column style={{ maxWidth: 450 }}>
          {/* Header ---------------------------------------------------*/}
          <span className='homepage-header'>
            <Header as='h3' color='teal' textAlign='center'>
              <Image
                size='massive'
                src='/assets/logo.png'
                alt='logo'
                style={{ marginBottom: 12 }}
              />{' '}
              Field Team Management
            </Header>
          </span>

          {/* Segment container -------------------------------------------*/}
          <Segment>
            {/* If user is logged in ------------------------ */}
            {isAuthenticated && user ? (
              <Fragment>
                <Header as='h2' content={`Welcome back ${user.displayName},`} />

                {/* If user is CLOCKED in ------------------ */}
                {user.currentlyClockedIn ? (
                  <Fragment>
                    <h3>Currently clocked in at:</h3>
                    <Header
                      as='h2'
                      color='teal'
                      style={{ marginBottom: '25px' }}
                    >
                      {user.clockedInTimestamp.moniker} -{' '}
                      {user.clockedInTimestamp.jobsite}
                    </Header>

                    <p>{showClockedInDate()}</p>

                    <span className='homepage-clock-out'>
                      <Button
                        onClick={() =>
                          clockOutUser(
                            user.clockedInTimestamp.moniker,
                            authDispatch
                          )
                        }
                        color='teal'
                      >
                        Clock Out
                      </Button>
                    </span>
                  </Fragment>
                ) : (
                  <Fragment>
                    {/* If user is NOT CLOCKED in ------------------ */}
                    <h3 className='homepage-h3'>
                      You are currently not clocked in.
                    </h3>
                    <p>
                      Last jobsite visited:{' '}
                      {user.lastJobsiteVisited &&
                        user.lastJobsiteVisited.moniker}{' '}
                      -{' '}
                      {user.lastJobsiteVisited &&
                        user.lastJobsiteVisited.jobsite}
                    </p>
                    <div style={{ maxWidth: '300px', margin: 'auto' }}>
                      <Search
                        placeholder='search jobsite...'
                        loading={loading}
                        onResultSelect={handleResultSelect}
                        onSearchChange={handleSearchChange}
                        results={results}
                        value={selectionValue}
                      />
                    </div>
                    <span className='homepage-clock-in'>
                      <Button
                        style={{ marginTop: '1rem' }}
                        onClick={() =>
                          clockInUser(selectionValue, authDispatch)
                        }
                        color='teal'
                      >
                        Clock In
                      </Button>
                    </span>
                  </Fragment>
                )}
                <br />

                {/* Bottom options ------------------ */}
                <Divider />
                <span className='homepage-timestamps'>
                  <Button
                    as={Link}
                    to='/timestamps/user'
                    style={{ width: '150px' }}
                  >
                    Timestamps
                  </Button>
                </span>
                <span className='homepage-log-out'>
                  <Button onClick={() => logoutUser(authDispatch)}>
                    Log Out
                  </Button>
                </span>
              </Fragment>
            ) : (
              // If user is NOT LOGGED IN ---------------------------
              <Fragment>
                <Header as='h2' content='Welcome!' />
                <Button
                  onClick={() => openModal(<LoginForm />, modalDispatch)}
                  to='/login'
                  size='huge'
                >
                  Login
                </Button>
              </Fragment>
            )}
          </Segment>
        </Grid.Column>
      </Grid>
    </Segment>
  );
};

export default TestHome;
